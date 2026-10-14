# 13 — Finding bottlenecks in .NET: counters, traces and benchmarks

Time: 90 minutes.

Goal: go from "k6 says it is slow" to "this line is why". Measure a running .NET app, and micro-benchmark a piece of code.

The job ad: "identify bottlenecks and stability risks, and work with engineering to drive measurable quality improvements". k6 (lesson 05) tells you **that** something is slow. This lesson tells you **where** and **why**.

## Learn: the tools

| Question | Tool | What you get |
| --- | --- | --- |
| Is the whole system slow under load? | k6, JMeter, NBomber | p95, throughput, errors |
| What is the process doing right now? | `dotnet-counters` | Live CPU, memory, GC, thread pool, exceptions, requests per second |
| Where does the time go? | `dotnet-trace` + PerfView / Visual Studio / speedscope; dotTrace in Rider | Call stacks: which methods use the time |
| Is memory growing? | `dotnet-gcdump`, `dotnet-dump`, Visual Studio memory tool, dotMemory | Objects on the heap |
| How fast is this one method? | BenchmarkDotNet | Mean time, allocations, compared across inputs |
| In production? | APM: Application Insights, Datadog, OpenTelemetry | Traces across services, slow queries |

Install: [tools/dotnet-diagnostics.md](../tools/dotnet-diagnostics.md).

## Learn: reading `dotnet-counters`

```bash
dotnet-counters ps                                    # find the API process id
dotnet-counters monitor -p <id> --counters System.Runtime,Microsoft.AspNetCore.Hosting
```

.NET 9 and later show the new metric names. Older articles use the old names in brackets.

| Counter | Healthy | Warning sign |
| --- | --- | --- |
| `dotnet.process.cpu.time` (CPU Usage) | Well below the number of cores under load | Near the limit: CPU-bound code (loops, serialization, regex) |
| `dotnet.thread_pool.queue.length` (ThreadPool Queue Length) | 0 or close to 0 | Keeps growing: **thread pool starvation**. Usually blocking calls (`Thread.Sleep`, `.Result`, sync I/O) |
| `dotnet.thread_pool.thread.count` (ThreadPool Thread Count) | Stable | Climbs slowly: the pool is adding threads to replace blocked ones |
| `dotnet.gc.last_collection.heap.size` (GC Heap Size) | Goes up and down | Only goes up during a soak test: a leak |
| `dotnet.gc.collections` with `gen2` (Gen 2 GC Count) | Rare | Frequent: too many long-lived objects |
| `dotnet.exceptions` (Exception Count) | Near 0 | Many: exceptions used for flow control, or hidden errors |

To save the numbers instead of watching them: `dotnet-counters collect -p <id> --counters System.Runtime --format csv -o counters.csv`.

## Learn: BenchmarkDotNet rules

```csharp
[MemoryDiagnoser]                      // also report memory allocated per call
public class TaperScheduleCalculatorBenchmarks
{
    [Params(1, 12, 52)]                // run once per value
    public int WeekCount { get; set; }

    [Benchmark]
    public TaperSchedule Calculate() => TaperScheduleCalculator.Calculate(40m, WeekCount);
}
```

- Always **Release** build, no debugger: `dotnet run -c Release --project benchmarks/DoseLab.Benchmarks -- --filter "*Calculator*"`.
- It warms up, runs many iterations, and reports **Mean**, **Error**, **StdDev**, and **Allocated**. Compare runs on the same machine only.
- `--job short` gives a quick, less exact result. Fine for learning.
- A micro-benchmark measures CPU and memory of code. It **cannot** see network or database time. Pick the tool that matches the cost.

## Learn: a bottleneck investigation, step by step

1. **Reproduce** with a load test and a baseline.
2. **Classify.** Gets worse with **more data** (N+1, missing index, no paging)? With **more users** (locks, thread pool, connection pool)? With **time** (leak)?
3. **Observe** with counters while the load runs.
4. **Locate** with a trace or profiler.
5. **Fix** together with the developer, change one thing.
6. **Confirm** with the same test, against the baseline. Add a guard (a threshold in CI, a unit test like L11-1) so it does not come back.

## In this repo

- `backend/benchmarks/DoseLab.Benchmarks/` — a sample benchmark and exercise L13-1.
- `SimulatedLatency` uses `Thread.Sleep`: a **blocking** call, on purpose. It stands in for a slow, synchronous database driver.
- Bug 07 (N+1) and bug 08 (race condition) from lesson 05.

## Do

Start the API from `backend` with `dotnet run -c Release --project src/DoseLab.Api`. Restart it before each load test.

1. **Benchmark sample.** Run the calculator benchmark. How do time and memory grow from 1 to 52 weeks? Is that what you expect from the code?
2. **L13-1.** Complete `TaperPlanListBenchmarks`. Run it with `--filter "*List*" --job short`. Then `instructor/bug.sh on 07`, run again, `instructor/bug.sh off 07`. Compare Mean and Allocated at 1000 plans. Now compare with what k6 showed for bug 07 in lesson 05. Why is the difference much bigger in k6? (Hint: what does `SimulatedLatency` add per call, and does the benchmark use it?)
3. **A healthy baseline.** Start `dotnet-counters monitor -p <id> --counters System.Runtime` on the API. In another terminal run your stress test from lesson 05 (`perf/stress-create.js`, 50 users). Write down the k6 p95 and the highest `dotnet.thread_pool.queue.length`. (Reference on a laptop: p95 about 13 ms, queue length 0.)
4. **Make it starve.** Stop the API. Start it with a slow "database": `Lab__SimulatedLatencyMs=50` (lesson 00 shows how). Each save now sleeps twice: about 100 ms of work. Run the same stress test while you watch the counters. Expected on a laptop: queue length goes up to 20 or more, and p95 about 300 ms, three times the work time. Explain it: every request **blocks** a thread in `Thread.Sleep`; the pool has too few threads, so new requests wait in the queue. The fix is not "more servers": it is async I/O (`await` instead of blocking), so a waiting request does not hold a thread.
5. **Trace (optional).** Collect a 20-second trace during the load: `dotnet-trace collect -p <id> --duration 00:00:00:20`. Open the `.nettrace` file in Visual Studio (File → Open) or PerfView. Find `Thread.Sleep` in the hot paths.
6. **Write it up.** Add a section to `notes/perf-report.md`: symptom, numbers before, evidence (counters, trace), cause, suggested fix (async database calls, or fewer calls), and how you would confirm the fix.

## Check yourself

1. What does `dotnet-counters` show that k6 cannot?
2. What is thread pool starvation, and which counter shows it?
3. Why must benchmarks run in Release, without a debugger?
4. A benchmark shows a change costs +27%, but k6 shows the endpoint is 50 times slower. How can both be true?
5. Name the six steps of a bottleneck investigation.

## Say it in the interview

- "k6 tells me that p95 is over target. Then I watch dotnet-counters during the load: a growing thread pool queue points at blocking calls; a heap that only grows points at a leak."
- "For hot code paths I use BenchmarkDotNet with a memory diagnoser, in Release, and I compare against a baseline."
- "After a fix, I rerun the same load test against the baseline, and I add a guard: a k6 threshold in CI or a unit test on the call count."
