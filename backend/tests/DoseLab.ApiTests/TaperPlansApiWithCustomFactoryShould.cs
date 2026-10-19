using System.Net;
using System.Net.Http.Json;
using DoseLab.ApiTests.Harness;
using FluentAssertions;
using Xunit;

namespace DoseLab.ApiTests;

// Lesson 11: the same API, started by a custom factory. Exercises: git grep "TODO(L11"
public class TaperPlansApiWithCustomFactoryShould(DoseLabApiFactory factory) : IClassFixture<DoseLabApiFactory>
{
    [Fact]
    public async Task StampANewPlanWithTheDateFromTheClock()
    {
        // setup

        var client = factory.CreateClientWithApiKey();
        factory.Clock.SetUtcNow(new DateTimeOffset(2026, 4, 30, 12, 0, 0, TimeSpan.Zero));

        // execute

        var response = await client.PostAsJsonAsync("/api/taper-plans",
            new { patientName = $"Clock {Guid.NewGuid():N}", medicationCode = "CLM", startingDailyDoseMg = 10, weekCount = 3 });

        // verify

        response.IsSuccessStatusCode.Should().BeTrue();
        var plan = await response.Content.ReadFromJsonAsync<PlanBody>();
        plan!.CreatedOn.Should().Be(new DateOnly(2026, 4, 30));
    }

    [Fact(Skip = "TODO(L11-5)")]
    public async Task RejectTheDefaultApiKeyWhenTheConfigurationUsesAnother()
    {
        // setup

        var client = factory.CreateClient();
        // TODO(L11-5): send the old key "lab-dev-key" in the X-Api-Key header.
        // The factory configured "test-key", so the old key must not work.

        // execute

        var response = await client.PostAsJsonAsync("/api/taper-plans",
            new { patientName = "Old Key", medicationCode = "CLM", startingDailyDoseMg = 10, weekCount = 3 });

        // verify

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        // TODO(L11-5): also prove nothing was saved: GET /api/taper-plans must not contain "Old Key".
    }

    private sealed record PlanBody(string ReferenceNumber, string PatientName, DateOnly CreatedOn);
}
