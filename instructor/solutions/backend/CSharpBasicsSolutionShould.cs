// Answer key for lesson 00. Copy into backend/tests/DoseLab.UnitTests/ to run it.
using DoseLab.Application;
using Xunit;

namespace DoseLab.UnitTests.Basics;

public class CSharpBasicsSolutionShould
{
    private readonly InMemoryMedicationCatalog _catalog = new(SimulatedLatency.None);

    [Fact]
    public void FilterAndProjectWithLinq()
    {
        // setup

        var medications = _catalog.List();

        // execute

        var codes = medications.Where(medication => medication.MaxDailyDoseMg > 30m).Select(medication => medication.Code);

        // verify

        Assert.Equal(["STR", "CLM"], codes);
    }

    [Fact]
    public void AddUpAListWithLinq()
    {
        // setup

        var schedule = TaperScheduleCalculator.Calculate(40m, 4);

        // execute

        var sum = schedule.Weeks.Sum(week => week.WeeklyTotalMg);

        // verify

        Assert.Equal(schedule.TotalMg, sum);
    }

    [Fact]
    public void BuildALookupWithADictionary()
    {
        // setup

        var medications = _catalog.List();

        // execute

        var namesByCode = medications.ToDictionary(medication => medication.Code, medication => medication.Name);

        // verify

        Assert.Equal("Nervalin", namesByCode["NRV"]);
        Assert.False(namesByCode.ContainsKey("XXX"));
    }

    [Fact]
    public void CompareRecordsByValueAndCopyThemWithWith()
    {
        // setup

        var calmafen = new Medication("CLM", "Calmafen", 40m);

        // execute

        var stronger = calmafen with { MaxDailyDoseMg = 60m };

        // verify

        Assert.Equal(new Medication("CLM", "Calmafen", 40m), calmafen);
        Assert.Equal(60m, stronger.MaxDailyDoseMg);
        Assert.Equal("Calmafen", stronger.Name);
    }

    [Fact]
    public void HandleAMissingValueWithoutCrashing()
    {
        // setup

        var code = "XXX";

        // execute

        var name = _catalog.Find(code)?.Name ?? "Unknown";

        // verify

        Assert.Equal("Unknown", name);
    }

    [Fact]
    public void CheckThatTheRightExceptionIsThrown()
    {
        // setup

        var weekCount = 0;

        // execute & verify

        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => TaperScheduleCalculator.Calculate(10m, weekCount));
        Assert.Equal("weekCount", exception.ParamName);
    }

    [Fact]
    public void ShowWhyDosesUseDecimalAndNotDouble()
    {
        // execute & verify

        Assert.NotEqual(0.3, 0.1 + 0.2);
        Assert.Equal(0.3m, 0.1m + 0.2m);
    }

    [Fact]
    public async Task RunWorkInParallelWithAsyncAndAwait()
    {
        // setup

        var service = new TaperPlanService(
            new InMemoryTaperPlanRepository(SimulatedLatency.None), _catalog, TimeProvider.System);
        string[] codes = ["STR", "CLM", "NRV"];

        // execute

        var schedules = await Task.WhenAll(codes.Select(code => Task.Run(() => service.Preview(code, 10m, 2))));

        // verify

        Assert.Equal(3, schedules.Length);
        Assert.All(schedules, schedule => Assert.Equal(5m, schedule.Weeks[1].DailyDoseMg));
    }
}
