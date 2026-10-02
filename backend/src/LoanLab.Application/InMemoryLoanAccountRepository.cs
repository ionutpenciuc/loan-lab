namespace LoanLab.Application;

public sealed class InMemoryLoanAccountRepository : ILoanAccountRepository
{
    private readonly List<LoanAccount> _accounts = new();
    private readonly object _gate = new();

    public IReadOnlyList<LoanAccount> List()
    {
        lock (_gate)
        {
            return _accounts.ToList();
        }
    }

    public void Add(LoanAccount account)
    {
        lock (_gate)
        {
            _accounts.Add(account);
        }
    }
}
