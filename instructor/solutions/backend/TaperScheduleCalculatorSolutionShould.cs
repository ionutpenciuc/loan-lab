// Answer key. Copy into backend/tests/DoseLab.UnitTests/ to run it.
using DoseLab.Application;
using Xunit;

namespace DoseLab.UnitTests;

public class TaperScheduleCalculatorSolutionShould
{
    // Catches bug 01. 10 / 3 does not divide evenly, so the last week must keep the remainder.
    [Fact]
    public void KeepTheRoundingRemainderInTheLastWeek()
    {
        // setup

        var startingDailyDoseMg = 10m;
        var weekCount = 3;

        // execute

        var schedule = TaperScheduleCalculator.Calculate(startingDailyDoseMg, weekCount);

        // verify

        Assert.Equal(10m, schedule.Weeks[0].DailyDoseMg);
        Assert.Equal(6.67m, schedule.Weeks[1].DailyDoseMg);
        Assert.Equal(3.34m, schedule.Weeks[2].DailyDoseMg);
        Assert.Equal(140.07m, schedule.TotalMg);
    }

    [Theory]
    [InlineData(40, 4, 10)]
    [InlineData(10, 3, 3.34)]
    [InlineData(1, 52, 0.49)]
    [InlineData(80, 1, 80)]
    public void EndOnAPositiveLastWeekDose(decimal startingDailyDoseMg, int weekCount, decimal expectedLastDose)
    {
        // setup

        // execute

        var schedule = TaperScheduleCalculator.Calculate(startingDailyDoseMg, weekCount);

        // verify

        Assert.Equal(weekCount, schedule.Weeks.Count);
        Assert.Equal(expectedLastDose, schedule.Weeks[^1].DailyDoseMg);
        Assert.True(schedule.Weeks[^1].DailyDoseMg > 0);
    }

    [Fact]
    public void RejectZeroWeeks()
    {
        // setup

        // execute & verify

        Assert.Throws<ArgumentOutOfRangeException>(() => TaperScheduleCalculator.Calculate(40m, 0));
    }
}
