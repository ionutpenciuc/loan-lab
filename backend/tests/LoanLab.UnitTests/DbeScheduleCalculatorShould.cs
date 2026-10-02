using LoanLab.Application;
using Xunit;

namespace LoanLab.UnitTests;

public class DbeScheduleCalculatorShould
{
    [Fact]
    public void BuildATwelveMonthScheduleWithEqualPrincipal()
    {
        // setup

        var amount = 1200m;
        var annualInterestRate = 12m;
        var installmentCount = 12;

        // execute

        var schedule = DbeScheduleCalculator.Calculate(amount, annualInterestRate, installmentCount);

        // verify

        Assert.Equal(12, schedule.Lines.Count);

        var first = schedule.Lines[0];
        Assert.Equal(1, first.Number);
        Assert.Equal(100m, first.Principal);
        Assert.Equal(12m, first.Interest);
        Assert.Equal(112m, first.Installment);
        Assert.Equal(1100m, first.RemainingBalance);

        var second = schedule.Lines[1];
        Assert.Equal(11m, second.Interest);
        Assert.Equal(111m, second.Installment);
        Assert.Equal(1000m, second.RemainingBalance);

        var last = schedule.Lines[11];
        Assert.Equal(12, last.Number);
        Assert.Equal(100m, last.Principal);
        Assert.Equal(1m, last.Interest);
        Assert.Equal(101m, last.Installment);
        Assert.Equal(0m, last.RemainingBalance);

        Assert.Equal(1200m, schedule.TotalPrincipal);
        Assert.Equal(78m, schedule.TotalInterest);
        Assert.Equal(1278m, schedule.TotalInstallment);
    }

    // Add your own [Fact] methods below this line.
}
