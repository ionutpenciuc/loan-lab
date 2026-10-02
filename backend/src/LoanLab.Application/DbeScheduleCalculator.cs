namespace LoanLab.Application;

public static class DbeScheduleCalculator
{
    public static LoanSchedule Calculate(decimal amount, decimal annualInterestRatePercent, int installmentCount)
    {
        if (installmentCount < 1)
            throw new ArgumentOutOfRangeException(nameof(installmentCount));

        var monthlyRate = annualInterestRatePercent / 100m / 12m;
        var regularPrincipal = RoundMoney(amount / installmentCount);
        var lines = new List<ScheduleLine>(installmentCount);
        var outstanding = amount;
        decimal totalInterest = 0m;

        for (var number = 1; number <= installmentCount; number++)
        {
            var principal = number == installmentCount
                ? outstanding
                : Math.Min(regularPrincipal, outstanding);
            var interest = RoundMoney(outstanding * monthlyRate);
            var installment = principal + interest;
            outstanding -= principal;
            totalInterest += interest;

            lines.Add(new ScheduleLine(number, principal, interest, installment, outstanding));
        }

        return new LoanSchedule(lines, amount, totalInterest, amount + totalInterest);
    }

    private static decimal RoundMoney(decimal value) =>
        decimal.Round(value, 2, MidpointRounding.AwayFromZero);
}
