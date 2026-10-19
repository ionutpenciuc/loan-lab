# .NET diagnostics: counters, traces, benchmarks

Needed for lesson 13.

## Install

```powershell
dotnet tool install --global dotnet-counters
dotnet tool install --global dotnet-trace
```

Close and reopen the terminal. Check: `dotnet-counters --version`.

## dotnet-counters

```powershell
dotnet-counters ps                                       # list .NET processes; find DoseLab.Api
dotnet-counters monitor -p <id> --counters System.Runtime
```

Press `q` to stop. To save the numbers: `dotnet-counters collect -p <id> --counters System.Runtime --format csv -o counters.csv`.

## dotnet-trace

```powershell
dotnet-trace collect -p <id> --duration 00:00:00:20
```

It writes a `.nettrace` file. Open it with:

- **Visual Studio**: File → Open → File, then look at the CPU usage / hot path view.
- **PerfView** (free, Microsoft): https://github.com/microsoft/perfview/releases
- **speedscope** (browser): first `dotnet-trace convert --format Speedscope file.nettrace`, then open https://www.speedscope.app

Rider users with a licence that includes **dotTrace** can profile from the IDE instead.

## BenchmarkDotNet

No install: it is a NuGet package in `backend/benchmarks/DoseLab.Benchmarks`. From `backend`:

```powershell
dotnet run -c Release --project benchmarks/DoseLab.Benchmarks -- --filter "*Calculator*"
dotnet run -c Release --project benchmarks/DoseLab.Benchmarks -- --filter "*" --job short    # quicker, less exact
```

Results are also saved in `BenchmarkDotNet.Artifacts/results/` as Markdown, CSV and HTML.

Close other heavy programs while benchmarks run. Plug in the laptop: battery saving changes CPU speed.

Docs: https://benchmarkdotnet.org/articles/overview.html
