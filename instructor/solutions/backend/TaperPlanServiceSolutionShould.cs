// Answer key. Copy into backend/tests/DoseLab.UnitTests/ to run it.
using DoseLab.Application;
using Xunit;

namespace DoseLab.UnitTests;

public class TaperPlanServiceSolutionShould
{
    private readonly InMemoryTaperPlanRepository _repository = new(SimulatedLatency.None);
    private readonly TaperPlanService _service;

    public TaperPlanServiceSolutionShould()
    {
        _service = new TaperPlanService(_repository, new InMemoryMedicationCatalog(SimulatedLatency.None), TimeProvider.System);
    }

    // Catches bug 02. Boundary value: exactly the maximum is allowed.
    [Fact]
    public void AcceptAStartingDoseEqualToTheMedicationMaximum()
    {
        // setup

        var sterivaMaxMg = 80m;

        // execute

        var plan = _service.Create("Ana Pop", "STR", sterivaMaxMg, 4);

        // verify

        Assert.Equal(80m, plan.Plan.StartingDailyDoseMg);
    }

    [Fact]
    public void RejectAStartingDoseJustAboveTheMedicationMaximum()
    {
        // setup

        // execute

        var error = Assert.Throws<TaperValidationException>(() => _service.Create("Ana Pop", "STR", 80.01m, 4));

        // verify

        Assert.Equal("Starting daily dose must not exceed 80 mg for Steriva.", error.Message);
        Assert.Empty(_service.List());
    }

    [Theory]
    [InlineData(0.99, false)]
    [InlineData(1, true)]
    [InlineData(20, true)]
    [InlineData(20.01, false)]
    public void ApplyTheDoseLimitsForNervalin(decimal startingDailyDoseMg, bool accepted)
    {
        // setup

        // execute

        var error = Record.Exception(() => _service.Preview("NRV", startingDailyDoseMg, 4));

        // verify

        Assert.Equal(accepted, error is null);
    }

    [Theory]
    [InlineData(0, false)]
    [InlineData(1, true)]
    [InlineData(52, true)]
    [InlineData(53, false)]
    public void ApplyTheWeekLimits(int weekCount, bool accepted)
    {
        // setup

        // execute

        var error = Record.Exception(() => _service.Preview("STR", 40m, weekCount));

        // verify

        Assert.Equal(accepted, error is null);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void RejectAMissingPatientName(string? patientName)
    {
        // setup

        // execute

        var error = Assert.Throws<TaperValidationException>(() => _service.Create(patientName, "STR", 40m, 4));

        // verify

        Assert.Equal("Patient name is required.", error.Message);
    }

    [Fact]
    public void RejectAnUnknownMedication()
    {
        // setup

        // execute

        var error = Assert.Throws<TaperValidationException>(() => _service.Create("Ana Pop", "XXX", 40m, 4));

        // verify

        Assert.Equal("Unknown medication.", error.Message);
    }

    [Fact]
    public void NotSaveAnythingOnPreview()
    {
        // setup

        // execute

        _service.Preview("STR", 40m, 4);

        // verify

        Assert.Empty(_service.List());
    }

    [Fact]
    public void GiveEachSavedPlanTheNextReferenceNumber()
    {
        // setup

        // execute

        var first = _service.Create("Ana Pop", "STR", 40m, 4);
        var second = _service.Create("Ion Pop", "CLM", 20m, 2);

        // verify

        Assert.Equal("TP-0001", first.Plan.ReferenceNumber);
        Assert.Equal("TP-0002", second.Plan.ReferenceNumber);
    }

    // A fake clock makes the created date predictable. No test should depend on today's date.
    [Fact]
    public void StampThePlanWithTheClockDate()
    {
        // setup

        var clock = new FixedClock(new DateTimeOffset(2026, 3, 9, 10, 0, 0, TimeSpan.Zero));
        var service = new TaperPlanService(_repository, new InMemoryMedicationCatalog(SimulatedLatency.None), clock);

        // execute

        var plan = service.Create("Ana Pop", "STR", 40m, 4);

        // verify

        Assert.Equal(new DateOnly(2026, 3, 9), plan.Plan.CreatedOn);
    }

    private sealed class FixedClock : TimeProvider
    {
        private readonly DateTimeOffset _now;

        public FixedClock(DateTimeOffset now)
        {
            _now = now;
        }

        public override DateTimeOffset GetUtcNow() => _now;
    }
}
