using System.Net.Http.Json;
using DoseLab.ApiTests.Harness;
using FluentAssertions;
using Xunit;

namespace DoseLab.ApiTests;

// Lesson 11, exercise TODO(L11-6): a flaky test in quarantine.
// 1. Remove the Skip. Run only that test:  dotnet test --filter "FullyQualifiedName~ListOnlyTheTwoSeededPlans"
// 2. Run the whole class:                   dotnet test --filter "FullyQualifiedName~FlakyExamplesShould"
// 3. Explain why the result changes. Fix the TEST (not the app), so it passes both ways. Then remove this comment.
public class FlakyExamplesShould(DoseLabApiFactory factory) : IClassFixture<DoseLabApiFactory>
{
    private readonly HttpClient _client = factory.CreateClientWithApiKey();

    [Fact]
    public async Task AcceptANewPlan()
    {
        // execute

        var response = await _client.PostAsJsonAsync("/api/taper-plans",
            new { patientName = "Flaky Example", medicationCode = "STR", startingDailyDoseMg = 40, weekCount = 4 });

        // verify

        response.IsSuccessStatusCode.Should().BeTrue();
    }

    [Fact(Skip = "Quarantined: fails or passes depending on the other tests. TODO(L11-6)")]
    public async Task ListOnlyTheTwoSeededPlans()
    {
        // execute

        var plans = await _client.GetFromJsonAsync<List<PlanBody>>("/api/taper-plans");

        // verify

        plans.Should().HaveCount(2);
    }

    private sealed record PlanBody(string ReferenceNumber, string PatientName);
}
