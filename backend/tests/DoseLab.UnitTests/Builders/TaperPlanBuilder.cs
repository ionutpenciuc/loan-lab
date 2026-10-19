using DoseLab.Application;

namespace DoseLab.UnitTests.Builders;

/// <summary>
/// Test data builder. Every plan is valid by default; a test only states the values it cares about.
/// <code>
/// var plan = TaperPlanBuilder.APlan().WithMedication("CLM").WithStartingDose(10m).Build();
/// var plans = TaperPlanBuilder.APlan().BuildMany(200);
/// </code>
/// </summary>
public sealed class TaperPlanBuilder
{
    private static int _next;

    private string _patientName = "Test Patient";
    private string _medicationCode = "STR";
    private decimal _startingDailyDoseMg = 40m;
    private int _weekCount = 4;
    private DateOnly _createdOn = new(2026, 1, 15);

    public static TaperPlanBuilder APlan() => new();

    public TaperPlanBuilder ForPatient(string patientName)
    {
        _patientName = patientName;
        return this;
    }

    public TaperPlanBuilder WithMedication(string medicationCode)
    {
        _medicationCode = medicationCode;
        return this;
    }

    public TaperPlanBuilder WithStartingDose(decimal startingDailyDoseMg)
    {
        _startingDailyDoseMg = startingDailyDoseMg;
        return this;
    }

    public TaperPlanBuilder OverWeeks(int weekCount)
    {
        _weekCount = weekCount;
        return this;
    }

    public TaperPlanBuilder CreatedOn(DateOnly createdOn)
    {
        _createdOn = createdOn;
        return this;
    }

    public TaperPlan Build()
    {
        var number = Interlocked.Increment(ref _next);
        return new TaperPlan(
            Guid.NewGuid(),
            $"TP-{number:0000}",
            _patientName,
            _medicationCode,
            _startingDailyDoseMg,
            _weekCount,
            _createdOn);
    }

    public IReadOnlyList<TaperPlan> BuildMany(int count) =>
        Enumerable.Range(0, count).Select(_ => Build()).ToList();

    public NewTaperPlan BuildDraft() =>
        new(_patientName, _medicationCode, _startingDailyDoseMg, _weekCount, _createdOn);
}
