using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace LoanLab.ApiTests;

public class LoanAccountsApiShould(WebApplicationFactory<Program> factory)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client = factory.CreateClient();

    [Fact]
    public async Task ReturnTheSeededLoanAccounts()
    {
        // execute

        var response = await _client.GetAsync("/api/loan-accounts");

        // verify

        response.EnsureSuccessStatusCode();
        var accounts = await response.Content.ReadFromJsonAsync<List<LoanAccountBody>>();

        Assert.NotNull(accounts);
        Assert.Contains(accounts, account => account.CustomerName == "Maria Ionescu");
        Assert.Contains(accounts, account => account.CustomerName == "Andrei Stan");
    }

    private sealed record LoanAccountBody(string CustomerName, decimal Amount, int InstallmentCount);
}
