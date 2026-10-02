namespace DoseLab.Application;

public sealed class InMemoryTaperPlanRepository : ITaperPlanRepository
{
    private readonly List<TaperPlan> _plans = new();
    private readonly object _gate = new();
    private readonly SimulatedLatency _latency;

    public InMemoryTaperPlanRepository(SimulatedLatency latency)
    {
        _latency = latency;
    }

    public IReadOnlyList<TaperPlan> List()
    {
        _latency.Wait();
        lock (_gate)
        {
            return _plans.ToList();
        }
    }

    public TaperPlan Add(NewTaperPlan draft)
    {
        _latency.Wait();
        lock (_gate)
        {
            var plan = new TaperPlan(
                Guid.NewGuid(),
                $"TP-{_plans.Count + 1:0000}",
                draft.PatientName,
                draft.MedicationCode,
                draft.StartingDailyDoseMg,
                draft.WeekCount,
                draft.CreatedOn);

            _plans.Add(plan);
            return plan;
        }
    }
}
