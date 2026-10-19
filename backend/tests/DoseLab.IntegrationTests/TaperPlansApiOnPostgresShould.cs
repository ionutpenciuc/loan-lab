using System.Net.Http.Json;
using DoseLab.IntegrationTests.Harness;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace DoseLab.IntegrationTests;

// Lesson 12: the whole API, configured to use a real PostgreSQL. Exercises: git grep "TODO(L12"
[Collection(PostgresCollection.Name)]
[Trait("Category", "Docker")]
public class TaperPlansApiOnPostgresShould(PostgresFixture postgres)
{
    [Fact]
    public async Task KeepASavedPlanAfterARestart()
    {
        // setup

        var connectionString = await postgres.CreateDatabaseAsync();
        var patientName = $"Restart {Guid.NewGuid():N}";

        await using (var firstRun = StartApi(connectionString))
        {
            var client = firstRun.CreateClient();
            client.DefaultRequestHeaders.Add("X-Api-Key", "lab-dev-key");
            var saved = await client.PostAsJsonAsync("/api/taper-plans",
                new { patientName, medicationCode = "CLM", startingDailyDoseMg = 10, weekCount = 3 });
            saved.IsSuccessStatusCode.Should().BeTrue();
        }

        // execute

        await using var secondRun = StartApi(connectionString);
        var plans = await secondRun.CreateClient().GetFromJsonAsync<List<PlanBody>>("/api/taper-plans");

        // verify

        plans.Should().ContainSingle(plan => plan.PatientName == patientName);
    }

    [Fact(Skip = "TODO(L12-4)")]
    public async Task SeedTheSamplePlansOnlyOnce()
    {
        // TODO(L12-4): start the API twice on the same new database (like the test above).
        // After the second start, "Maria Ionescu" must be in the list exactly once.
        await Task.CompletedTask;
        Assert.Fail("Not written yet.");
    }

    private static WebApplicationFactory<Program> StartApi(string connectionString) =>
        new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseSetting("Lab:Storage", "Postgres");
            builder.UseSetting("ConnectionStrings:DoseLab", connectionString);
            builder.UseSetting("Lab:SimulatedLatencyMs", "0");
        });

    private sealed record PlanBody(string ReferenceNumber, string PatientName);
}
