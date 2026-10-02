using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace DoseLab.ApiTests;

public class TaperPlansApiShould : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public TaperPlansApiShould(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task ReturnTheSeededTaperPlans()
    {
        // setup

        // execute

        var response = await _client.GetAsync("/api/taper-plans");

        // verify

        response.EnsureSuccessStatusCode();
        var plans = await response.Content.ReadFromJsonAsync<List<TaperPlanBody>>();

        Assert.NotNull(plans);
        Assert.Contains(plans, plan => plan.PatientName == "Maria Ionescu" && plan.ReferenceNumber == "TP-0001");
        Assert.Contains(plans, plan => plan.PatientName == "Andrei Stan" && plan.ReferenceNumber == "TP-0002");
    }

    // Add your own [Fact] methods below this line.

    private sealed record TaperPlanBody(string ReferenceNumber, string PatientName, string MedicationName);
}
