using LoanLab.Application;
using Xunit;

namespace LoanLab.UnitTests;

public class LoanAccountServiceShould
{
    [Fact]
    public void SaveALoanThatAppearsInTheList()
    {
        // setup

        var repository = new InMemoryLoanAccountRepository();
        var service = new LoanAccountService(repository);

        // execute

        service.Create("Ana Pop", 1200m, 12m, 12);

        // verify

        var saved = Assert.Single(service.List());
        Assert.Equal("Ana Pop", saved.CustomerName);
        Assert.Equal(1200m, saved.Amount);
        Assert.Equal(12m, saved.AnnualInterestRate);
        Assert.Equal(12, saved.InstallmentCount);
    }

    // Add your own [Fact] methods below this line.
}
