namespace LoanLab.Application;

public sealed class LoanAccountService
{
    private readonly ILoanAccountRepository _repository;

    public LoanAccountService(ILoanAccountRepository repository)
    {
        _repository = repository;
    }

    public IReadOnlyList<LoanAccount> List() => _repository.List();

    public LoanSchedule Preview(decimal amount, decimal annualInterestRate, int installmentCount)
    {
        ValidateSchedule(amount, annualInterestRate, installmentCount);
        return DbeScheduleCalculator.Calculate(amount, annualInterestRate, installmentCount);
    }

    public LoanAccount Create(string? customerName, decimal amount, decimal annualInterestRate, int installmentCount)
    {
        var name = customerName?.Trim() ?? "";
        if (name.Length == 0)
            throw new LoanValidationException("Customer name is required.");
        if (name.Length > 100)
            throw new LoanValidationException("Customer name must be 100 characters or fewer.");

        ValidateSchedule(amount, annualInterestRate, installmentCount);

        var account = new LoanAccount(
            Guid.NewGuid(),
            name,
            amount,
            annualInterestRate,
            installmentCount,
            DateOnly.FromDateTime(DateTime.UtcNow));

        _repository.Add(account);
        return account;
    }

    private static void ValidateSchedule(decimal amount, decimal annualInterestRate, int installmentCount)
    {
        if (amount < 0.01m)
            throw new LoanValidationException("Loan amount must be at least 0.01.");
        if (annualInterestRate < 0m || annualInterestRate > 100m)
            throw new LoanValidationException("Annual interest rate must be between 0 and 100.");
        if (installmentCount < 1 || installmentCount > 360)
            throw new LoanValidationException("Number of installments must be between 1 and 360.");
    }
}
