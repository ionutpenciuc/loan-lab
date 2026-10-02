using DoseLab.Application;
using Xunit;

namespace DoseLab.UnitTests;

public class TaperScheduleCalculatorShould
{
    [Fact]
    public void LowerTheDailyDoseByTheSameStepEveryWeek()
    {
        // setup

        var startingDailyDoseMg = 40m;
        var weekCount = 4;

        // execute

        var schedule = TaperScheduleCalculator.Calculate(startingDailyDoseMg, weekCount);

        // verify

        Assert.Equal(4, schedule.Weeks.Count);

        Assert.Equal(1, schedule.Weeks[0].Week);
        Assert.Equal(40m, schedule.Weeks[0].DailyDoseMg);
        Assert.Equal(280m, schedule.Weeks[0].WeeklyTotalMg);
        Assert.Equal(280m, schedule.Weeks[0].CumulativeTotalMg);

        Assert.Equal(30m, schedule.Weeks[1].DailyDoseMg);
        Assert.Equal(20m, schedule.Weeks[2].DailyDoseMg);

        Assert.Equal(4, schedule.Weeks[3].Week);
        Assert.Equal(10m, schedule.Weeks[3].DailyDoseMg);
        Assert.Equal(70m, schedule.Weeks[3].WeeklyTotalMg);
        Assert.Equal(700m, schedule.Weeks[3].CumulativeTotalMg);

        Assert.Equal(700m, schedule.TotalMg);
    }

    // Add your own [Fact] or [Theory] methods below this line.
}
