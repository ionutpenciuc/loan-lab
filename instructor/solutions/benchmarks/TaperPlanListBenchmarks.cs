using BenchmarkDotNet.Attributes;
using DoseLab.Application;

namespace DoseLab.Benchmarks;

// Answer key for lesson 13. Replace backend/benchmarks/DoseLab.Benchmarks/TaperPlanListBenchmarks.cs with this file.
// Measures TaperPlanService.List() as the number of saved plans grows.
[MemoryDiagnoser]
public class TaperPlanListBenchmarks
{
    private TaperPlanService _service = null!;

    [Params(10, 1000)]
    public int PlanCount { get; set; }

    [GlobalSetup]
    public void Setup()
    {
        // GlobalSetup runs once per parameter value, before the measurements. It is not measured.
        var repository = new InMemoryTaperPlanRepository(SimulatedLatency.None);
        _service = new TaperPlanService(repository, new InMemoryMedicationCatalog(SimulatedLatency.None), TimeProvider.System);

        for (var i = 0; i < PlanCount; i++)
            _service.Create($"Patient {i}", "CLM", 10m, 3);
    }

    [Benchmark]
    public int List() => _service.List().Count;
}
