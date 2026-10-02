# 05 — Performance and stress tests with k6

Time: 75 minutes.

Goal: write load and stress tests with thresholds, read the results, and find bottlenecks and stability risks.

## Learn

### Kinds of performance test

| Type | Question | Shape |
| --- | --- | --- |
| Smoke | Does it work at all under a tiny load? | 1 user, a few requests |
| Load | Does it meet targets at normal, expected traffic? | Expected users, 10–60 min |
| Stress | Where does it break, and **how** does it fail? | Ramp above normal until errors or slowdowns |
| Spike | Can it survive a sudden jump? | 0 → many users in seconds |
| Soak (endurance) | Does it degrade over time? Memory leaks? | Normal load for hours |
| Volume | Does it cope with a lot of data? | Large data set, normal load |

### Metrics

- **Response time percentiles.** p95 = 95% of requests were this fast or faster. Use p95 and p99, not the average. The average hides the slow tail that users feel.
- **Throughput.** Requests per second (RPS).
- **Error rate.** Failed requests ÷ all requests.
- **Concurrency.** Virtual users (VUs) at the same time.
- **Resources.** CPU, memory, garbage collection, thread pool, database connections.

A **threshold** (or SLO, service-level objective) turns a measurement into pass/fail: "p95 < 250 ms and errors < 1%". k6 exits with code 99 when a threshold fails, so CI can block the change.

### Good practice

- Test on an environment like production. Compare runs on the same environment.
- Record a **baseline**. Compare every run to it.
- Warm up first. Ignore the first seconds.
- Add **think time** (`sleep`) to act like real users.
- Know the difference between a **closed model** (fixed users, `constant-vus`) and an **open model** (fixed arrival rate, `constant-arrival-rate`). The open model shows queueing better.
- Change one thing at a time.

### Common bottlenecks

| Bottleneck | Symptom |
| --- | --- |
| N+1 queries: one database call per row | Endpoint gets slower as data grows |
| Blocking calls (`Thread.Sleep`, sync I/O) | Thread pool starvation: latency jumps under concurrency |
| Locks held too long | Throughput stops growing as you add users |
| Missing database index | Slow queries on big tables |
| Large responses, no paging | Slow and memory-heavy as data grows |
| Memory leak | Soak test: memory and latency grow over hours |

### Stability risks under concurrency

Some bugs only appear when requests run **at the same time**: race conditions, deadlocks, duplicate ids, lost updates. One user never triggers them. A stress test with many users does. In medical software, a duplicate prescription reference is a patient safety risk.

To find the cause: a profiler (dotTrace in Rider, Visual Studio Profiler), `dotnet-counters` (live CPU, GC, thread pool), `dotnet-trace`, logs, and APM tools (Application Performance Monitoring: Application Insights, Datadog, New Relic).

### The fake database

This app keeps data in memory. To act like a real database, every repository or catalog call waits 3 ms (`SimulatedLatency`). Change it in `backend/src/DoseLab.Api/appsettings.json` (`Lab:SimulatedLatencyMs`).

## Learn: k6

```js
import http from 'k6/http'
import { check, sleep } from 'k6'

export const options = {
  scenarios: {
    read_list: { executor: 'constant-vus', vus: 10, duration: '15s' },
  },
  thresholds: {
    http_req_failed: ['rate<0.01'],                    // under 1% errors
    'http_req_duration{name:list}': ['p(95)<250'],     // p95 of requests tagged "list"
  },
}

export function setup() {
  // runs once, before the load. Create test data here.
}

export default function () {
  // runs again and again, by every virtual user
  const response = http.get('http://localhost:5080/api/taper-plans', { tags: { name: 'list' } })
  check(response, { 'status is 200': (r) => r.status === 200 })   // a check records pass/fail; it does not stop the test
  sleep(0.1)
}

export function teardown(data) {
  // runs once, after the load. Verify the final state here.
}
```

- `check` = a soft assertion, counted in the summary.
- `threshold` = the pass/fail gate for the whole run.
- Executors: `constant-vus`, `ramping-vus` (stages), `constant-arrival-rate`, `ramping-arrival-rate`.
- Custom metrics: `Counter`, `Rate`, `Trend` from `k6/metrics`.
- Pass values from outside: `k6 run -e API_URL=http://localhost:5081 script.js`, read with `__ENV.API_URL`.

Other tools: JMeter (GUI, older), Gatling (Scala/Java), Locust (Python), NBomber (C#), Azure Load Testing (runs JMeter or Locust).

## Do

Install k6: https://grafana.com/docs/k6/latest/set-up/install-k6/ (macOS: `brew install k6`, Windows: `winget install k6 --source winget`).

Start the API first (`dotnet run --project src/DoseLab.Api` in `backend`). **Restart the API before each new test.** Data stays in memory and grows.

1. **Smoke.** `k6 run perf/smoke.js`. Read the summary: `http_req_duration`, p(95), `http_req_failed`, checks.
2. **Load test for the list.** Write `perf/list-load.js`:
   - `setup()` creates 200 plans with `POST /api/taper-plans` (header `X-Api-Key: lab-dev-key`).
   - 10 VUs for 15 s read `GET /api/taper-plans`, tagged `list`.
   - Thresholds: errors < 1%, p95 of `list` < 250 ms.
3. **Stress test for saving.** Write `perf/stress-create.js`:
   - `ramping-vus`: 1 → 50 VUs in 5 s, hold 50 for 10 s, down to 0 in 5 s.
   - Each iteration creates a plan with a unique name (`Stress ${__VU}-${__ITER}`), then `sleep(0.1)`.
   - `teardown()` reads all plans and counts **duplicate reference numbers**. Put the count in a `Counter` called `duplicate_references` with threshold `count==0`.
4. **Feel the latency.** Set `Lab:SimulatedLatencyMs` to 20. Restart. Run test 2 again. What changed? Put it back to 3.
5. **Write a performance report** in `notes/perf-report.md`: environment, scenario, load shape, results (p95, RPS, error rate), thresholds pass/fail, bottleneck found (if any), next step.

## Check yourself

1. Load vs stress vs spike vs soak: one sentence each.
2. Why p95 and not the average?
3. What is a threshold, and why does CI care?
4. An endpoint is fast with 2 rows and slow with 500. What is a likely cause?
5. Why can a bug appear only with 50 users at once?
6. Which tools would you use to find where the time goes?

## Say it in the interview

- "I start with a smoke test, then load at expected traffic, then stress to find the breaking point and how it fails."
- "I gate on p95 and error rate thresholds. k6 fails the pipeline when a threshold fails."
- "Data-dependent slowness usually means N+1 calls or a missing index. Concurrency bugs like duplicate ids only show under parallel load."
- "I run a short performance smoke on every merge, and the longer load and soak tests nightly."
