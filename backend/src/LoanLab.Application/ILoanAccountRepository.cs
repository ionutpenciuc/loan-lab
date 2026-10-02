namespace LoanLab.Application;

public interface ILoanAccountRepository
{
    IReadOnlyList<LoanAccount> List();

    void Add(LoanAccount account);
}
