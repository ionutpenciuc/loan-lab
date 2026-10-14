using DoseLab.Application;
using Xunit;

namespace DoseLab.UnitTests.Basics;

// Lesson 00 exercises. Each test is skipped and red on purpose.
// For each one: remove `Skip = ...`, run it and see it fail, then replace the TODO line in "execute" until it passes.
// Do not change the "verify" part. Find them all with: git grep "TODO(L00"
public class CSharpBasicsShould
{
    private readonly InMemoryMedicationCatalog _catalog = new(SimulatedLatency.None);

    [Fact(Skip = "TODO(L00-1)")]
    public void FilterAndProjectWithLinq()
    {
        // setup

        var medications = _catalog.List();

        // execute

        // TODO(L00-1): codes of the medications whose MaxDailyDoseMg is above 30. Use .Where(...) and .Select(...).
        IEnumerable<string> codes = [];

        // verify

        Assert.Equal(["STR", "CLM"], codes);
    }

    [Fact(Skip = "TODO(L00-2)")]
    public void AddUpAListWithLinq()
    {
        // setup

        var schedule = TaperScheduleCalculator.Calculate(40m, 4);

        // execute

        // TODO(L00-2): the sum of every week's WeeklyTotalMg. Use .Sum(week => ...).
        decimal sum = 0m;

        // verify

        Assert.Equal(schedule.TotalMg, sum);
    }

    [Fact(Skip = "TODO(L00-3)")]
    public void BuildALookupWithADictionary()
    {
        // setup

        var medications = _catalog.List();

        // execute

        // TODO(L00-3): a dictionary from code to name. Use .ToDictionary(medication => ..., medication => ...).
        Dictionary<string, string> namesByCode = new();

        // verify

        Assert.Equal("Nervalin", namesByCode["NRV"]);
        Assert.False(namesByCode.ContainsKey("XXX"));
    }

    [Fact(Skip = "TODO(L00-4)")]
    public void CompareRecordsByValueAndCopyThemWithWith()
    {
        // setup

        var calmafen = new Medication("CLM", "Calmafen", 40m);

        // execute

        // TODO(L00-4): make a copy of calmafen with MaxDailyDoseMg = 60. Use a `with` expression.
        Medication stronger = calmafen;

        // verify

        Assert.Equal(new Medication("CLM", "Calmafen", 40m), calmafen);   // records compare by value, not by reference
        Assert.Equal(60m, stronger.MaxDailyDoseMg);
        Assert.Equal("Calmafen", stronger.Name);
    }

    [Fact(Skip = "TODO(L00-5)")]
    public void HandleAMissingValueWithoutCrashing()
    {
        // setup

        var code = "XXX";

        // execute

        // TODO(L00-5): the medication name, or "Unknown" when Find returns null. Use ?. and ??.
        string name = _catalog.Find(code)!.Name;

        // verify

        Assert.Equal("Unknown", name);
    }

    [Fact(Skip = "TODO(L00-6)")]
    public void CheckThatTheRightExceptionIsThrown()
    {
        // setup

        var weekCount = 0;

        // execute & verify

        // TODO(L00-6): replace the line below. Assert that Calculate(10m, weekCount) throws ArgumentOutOfRangeException,
        // and that its ParamName is "weekCount". Hint: var exception = Assert.Throws<...>(() => ...);
        Assert.Fail($"Not written yet. weekCount is {weekCount}.");
    }

    [Fact(Skip = "TODO(L00-7)")]
    public void ShowWhyDosesUseDecimalAndNotDouble()
    {
        // execute & verify

        // TODO(L00-7): replace the line below with two asserts.
        // 1. Assert.NotEqual(0.3, 0.1 + 0.2)       double: binary rounding error
        // 2. Assert.Equal(0.3m, 0.1m + 0.2m)       decimal: exact
        // Then print 0.1 + 0.2 with: Console.WriteLine((0.1 + 0.2).ToString("R")); and run with: dotnet test --logger "console;verbosity=detailed"
        Assert.Fail("Not written yet.");
    }

    [Fact(Skip = "TODO(L00-8)")]
    public async Task RunWorkInParallelWithAsyncAndAwait()
    {
        // setup

        var service = new TaperPlanService(
            new InMemoryTaperPlanRepository(SimulatedLatency.None), _catalog, TimeProvider.System);
        string[] codes = ["STR", "CLM", "NRV"];

        // execute

        // TODO(L00-8): start one Task.Run(() => service.Preview(code, 10m, 2)) per code, then await Task.WhenAll(...).
        TaperSchedule[] schedules = await Task.FromResult(Array.Empty<TaperSchedule>());

        // verify

        Assert.Equal(3, schedules.Length);
        Assert.All(schedules, schedule => Assert.Equal(5m, schedule.Weeks[1].DailyDoseMg));
    }
}
