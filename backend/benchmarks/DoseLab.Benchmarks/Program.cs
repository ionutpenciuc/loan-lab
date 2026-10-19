using BenchmarkDotNet.Running;

// Always run in Release, without a debugger:
//   dotnet run -c Release --project benchmarks/DoseLab.Benchmarks -- --filter "*"
// Pick one class with a filter, for example --filter "*Calculator*". Results: BenchmarkDotNet.Artifacts/results/
BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
