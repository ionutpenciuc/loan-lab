namespace DoseLab.Application;

/// <summary>
/// Equal-step taper: the daily dose drops by the same amount every week.
/// The step is rounded down to 0.01 mg, so the last week keeps the remainder
/// and the steps add up to the starting dose.
/// </summary>
public static class TaperScheduleCalculator
{
    public static TaperSchedule Calculate(decimal startingDailyDoseMg, int weekCount)
    {
        if (weekCount < 1)
            throw new ArgumentOutOfRangeException(nameof(weekCount));

        var step = Math.Floor(startingDailyDoseMg / weekCount * 100m) / 100m;
        var weeks = new List<ScheduleWeek>(weekCount);
        decimal cumulative = 0m;

        for (var week = 1; week <= weekCount; week++)
        {
            var dailyDose = startingDailyDoseMg - (week - 1) * step;
            var weeklyTotal = dailyDose * 7;
            cumulative += weeklyTotal;

            weeks.Add(new ScheduleWeek(week, dailyDose, weeklyTotal, cumulative));
        }

        return new TaperSchedule(weeks, cumulative);
    }
}
