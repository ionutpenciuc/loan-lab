// Answer key for lesson 11 (API level). Copy into backend/tests/DoseLab.ApiTests/ to run it.
//
// L11-6, why FlakyExamplesShould.ListOnlyTheTwoSeededPlans is flaky:
// both tests share one API (IClassFixture), so data saved by AcceptANewPlan is still there.
// Alone it sees 2 plans; after AcceptANewPlan it sees 3. Its result depends on test order.
// Fix: do not assert an exact count of shared data. Assert what this test needs (the seeded plans exist),
// or give the test its own API (a new factory) if it really needs an empty start.
using System.Net;
using System.Net.Http.Json;
using DoseLab.ApiTests.Harness;
using FluentAssertions;
using Xunit;

namespace DoseLab.ApiTests;

public class TaperPlansApiHarnessSolutionShould(DoseLabApiFactory factory) : IClassFixture<DoseLabApiFactory>
{
    [Fact]
    public async Task RejectTheDefaultApiKeyWhenTheConfigurationUsesAnother()
    {
        // setup

        var client = factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Api-Key", "lab-dev-key");

        // execute

        var response = await client.PostAsJsonAsync("/api/taper-plans",
            new { patientName = "Old Key", medicationCode = "CLM", startingDailyDoseMg = 10, weekCount = 3 });

        // verify

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        var plans = await client.GetFromJsonAsync<List<PlanBody>>("/api/taper-plans");
        plans.Should().NotContain(plan => plan.PatientName == "Old Key");
    }

    [Fact]
    public async Task ListTheTwoSeededPlans()
    {
        // setup

        var client = factory.CreateClientWithApiKey();
        await client.PostAsJsonAsync("/api/taper-plans",
            new { patientName = "Other Test Data", medicationCode = "STR", startingDailyDoseMg = 40, weekCount = 4 });

        // execute

        var plans = await client.GetFromJsonAsync<List<PlanBody>>("/api/taper-plans");

        // verify

        plans.Should().Contain(plan => plan.PatientName == "Maria Ionescu" && plan.ReferenceNumber == "TP-0001");
        plans.Should().Contain(plan => plan.PatientName == "Andrei Stan" && plan.ReferenceNumber == "TP-0002");
    }

    private sealed record PlanBody(string ReferenceNumber, string PatientName);
}
