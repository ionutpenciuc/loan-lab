using BenchmarkDotNet.Attributes;
using DoseLab.Application;

namespace DoseLab.Benchmarks;

// Lesson 13 exercise TODO(L13-1): measure TaperPlanService.List() as the number of saved plans grows.
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

        // TODO(L13-1): save PlanCount plans with _service.Create(...). Use a valid medication, dose and week count.
    }

    [Benchmark]
    public int List() => _service.List().Count;
}
