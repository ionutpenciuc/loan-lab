namespace DoseLab.Application;

public sealed class TaperPlanService
{
    public const decimal MinimumStartingDoseMg = 1m;
    public const int MaximumWeekCount = 52;

    private readonly ITaperPlanRepository _repository;
    private readonly IMedicationCatalog _catalog;
    private readonly TimeProvider _clock;

    public TaperPlanService(ITaperPlanRepository repository, IMedicationCatalog catalog, TimeProvider clock)
    {
        _repository = repository;
        _catalog = catalog;
        _clock = clock;
    }

    public IReadOnlyList<Medication> Medications() => _catalog.List();

    public IReadOnlyList<TaperPlanSummary> List()
    {
        var plans = _repository.List();
        var names = _catalog.List().ToDictionary(medication => medication.Code, medication => medication.Name);

        return plans
            .Select(plan => new TaperPlanSummary(plan, names.GetValueOrDefault(plan.MedicationCode, plan.MedicationCode)))
            .ToList();
    }

    public TaperPlanSummary? Find(Guid id)
    {
        var plan = _repository.List().FirstOrDefault(candidate => candidate.Id == id);
        if (plan is null)
            return null;

        var medication = _catalog.Find(plan.MedicationCode);
        return new TaperPlanSummary(plan, medication?.Name ?? plan.MedicationCode);
    }

    public TaperSchedule Preview(string? medicationCode, decimal startingDailyDoseMg, int weekCount)
    {
        ValidateDose(medicationCode, startingDailyDoseMg, weekCount);
        return TaperScheduleCalculator.Calculate(startingDailyDoseMg, weekCount);
    }

    public TaperPlanSummary Create(string? patientName, string? medicationCode, decimal startingDailyDoseMg, int weekCount)
    {
        var name = patientName?.Trim() ?? "";
        if (name.Length == 0)
            throw new TaperValidationException("Patient name is required.");
        if (name.Length > 100)
            throw new TaperValidationException("Patient name must be 100 characters or fewer.");

        var medication = ValidateDose(medicationCode, startingDailyDoseMg, weekCount);

        var plan = _repository.Add(new NewTaperPlan(
            name,
            medication.Code,
            startingDailyDoseMg,
            weekCount,
            DateOnly.FromDateTime(_clock.GetUtcNow().UtcDateTime)));

        return new TaperPlanSummary(plan, medication.Name);
    }

    private Medication ValidateDose(string? medicationCode, decimal startingDailyDoseMg, int weekCount)
    {
        var medication = _catalog.Find(medicationCode ?? "")
            ?? throw new TaperValidationException("Unknown medication.");

        if (startingDailyDoseMg < MinimumStartingDoseMg)
            throw new TaperValidationException("Starting daily dose must be at least 1 mg.");
        if (startingDailyDoseMg > medication.MaxDailyDoseMg)
            throw new TaperValidationException(
                $"Starting daily dose must not exceed {medication.MaxDailyDoseMg:0.##} mg for {medication.Name}.");
        if (weekCount < 1 || weekCount > MaximumWeekCount)
            throw new TaperValidationException("Number of weeks must be between 1 and 52.");

        return medication;
    }
}
