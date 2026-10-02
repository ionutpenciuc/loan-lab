// Answer key. Copy into backend/tests/DoseLab.ApiTests/ to run it.
using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace DoseLab.ApiTests;

public class TaperPlansApiSolutionShould : IClassFixture<WebApplicationFactory<Program>>
{
    private const string ApiKey = "lab-dev-key";

    private readonly HttpClient _client;

    public TaperPlansApiSolutionShould(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    // Catches bug 03.
    [Fact]
    public async Task ReturnCreatedWithALocationThatFindsThePlan()
    {
        // setup

        var request = CreateRequest(new { patientName = "Elena Radu", medicationCode = "STR", startingDailyDoseMg = 40, weekCount = 4 }, ApiKey);

        // execute

        var response = await _client.SendAsync(request);

        // verify

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);

        var created = await response.Content.ReadFromJsonAsync<PlanBody>();
        var fetched = await _client.GetFromJsonAsync<PlanBody>(response.Headers.Location);
        Assert.Equal(created!.Id, fetched!.Id);
        Assert.Equal("Elena Radu", fetched.PatientName);
    }

    // Catches bug 04.
    [Fact]
    public async Task RejectASaveWithoutAnApiKey()
    {
        // setup

        var request = CreateRequest(new { patientName = "No Key", medicationCode = "STR", startingDailyDoseMg = 40, weekCount = 4 }, apiKey: null);

        // execute

        var response = await _client.SendAsync(request);

        // verify

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        var plans = await _client.GetFromJsonAsync<List<PlanBody>>("/api/taper-plans");
        Assert.DoesNotContain(plans!, plan => plan.PatientName == "No Key");
    }

    [Fact]
    public async Task RejectASaveWithAWrongApiKey()
    {
        // setup

        var request = CreateRequest(new { patientName = "Wrong Key", medicationCode = "STR", startingDailyDoseMg = 40, weekCount = 4 }, "guess");

        // execute

        var response = await _client.SendAsync(request);

        // verify

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task ReturnBadRequestWithAMessageForAnInvalidPlan()
    {
        // setup

        var request = CreateRequest(new { patientName = "Ana Pop", medicationCode = "NRV", startingDailyDoseMg = 25, weekCount = 4 }, ApiKey);

        // execute

        var response = await _client.SendAsync(request);

        // verify

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        var error = await response.Content.ReadFromJsonAsync<ErrorBody>();
        Assert.Equal("Starting daily dose must not exceed 20 mg for Nervalin.", error!.Message);
    }

    [Fact]
    public async Task ReturnNotFoundForAnUnknownPlan()
    {
        // setup

        // execute

        var response = await _client.GetAsync($"/api/taper-plans/{Guid.NewGuid()}");

        // verify

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task PreviewWithoutSaving()
    {
        // setup

        var before = await _client.GetFromJsonAsync<List<PlanBody>>("/api/taper-plans");

        // execute

        var response = await _client.PostAsJsonAsync("/api/taper-plans/preview", new { medicationCode = "CLM", startingDailyDoseMg = 10, weekCount = 3 });

        // verify

        response.EnsureSuccessStatusCode();
        var after = await _client.GetFromJsonAsync<List<PlanBody>>("/api/taper-plans");
        Assert.Equal(before!.Count, after!.Count);
    }

    private static HttpRequestMessage CreateRequest(object body, string? apiKey)
    {
        var request = new HttpRequestMessage(HttpMethod.Post, "/api/taper-plans")
        {
            Content = JsonContent.Create(body),
        };
        if (apiKey is not null)
            request.Headers.Add("X-Api-Key", apiKey);
        return request;
    }

    private sealed record PlanBody(Guid Id, string ReferenceNumber, string PatientName);

    private sealed record ErrorBody(string Message);
}
