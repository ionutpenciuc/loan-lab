using LoanLab.Application;

namespace LoanLab.Api;

public static class LoanAccountSeed
{
    public static void Apply(ILoanAccountRepository repository)
    {
        repository.Add(new LoanAccount(
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            "Maria Ionescu",
            10000m,
            10m,
            10,
            new DateOnly(2026, 1, 15)));

        repository.Add(new LoanAccount(
            Guid.Parse("22222222-2222-2222-2222-222222222222"),
            "Andrei Stan",
            5000m,
            0m,
            5,
            new DateOnly(2026, 2, 1)));
    }
}
