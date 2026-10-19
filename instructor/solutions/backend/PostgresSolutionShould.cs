// Answer key for lesson 12. Copy into backend/tests/DoseLab.IntegrationTests/ to run it. Needs Docker.
// StoreAPatientNameExactlyAsWritten catches bug 09 (SQL injection): the name O'Brien breaks the INSERT.
using System.Globalization;
using DoseLab.Application;
using DoseLab.IntegrationTests.Harness;
using DoseLab.Postgres;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Npgsql;
using System.Net.Http.Json;
using Xunit;

namespace DoseLab.IntegrationTests;

[Collection(PostgresCollection.Name)]
[Trait("Category", "Docker")]
public class PostgresSolutionShould(PostgresFixture postgres) : IAsyncLifetime
{
    private NpgsqlDataSource _dataSource = null!;
    private PostgresTaperPlanRepository _repository = null!;

    public async Task InitializeAsync()
    {
        _dataSource = NpgsqlDataSource.Create(await postgres.CreateDatabaseAsync());
        _repository = new PostgresTaperPlanRepository(_dataSource);
        _repository.EnsureSchema();
    }

    public Task DisposeAsync() => _dataSource.DisposeAsync().AsTask();

    [Theory]
    [InlineData("6.67")]
    [InlineData("12.345")]
    [InlineData("80")]
    [InlineData("1")]
    [InlineData("0.1")]
    public void KeepTheStartingDoseExactly(string dose)
    {
        // setup

        var startingDailyDoseMg = decimal.Parse(dose, CultureInfo.InvariantCulture);

        // execute

        _repository.Add(new NewTaperPlan("Precise", "STR", startingDailyDoseMg, 4, new DateOnly(2026, 3, 1)));

        // verify

        _repository.List().Should().ContainSingle().Which.StartingDailyDoseMg.Should().Be(startingDailyDoseMg);
    }

    [Theory]
    [InlineData("Sean O'Brien")]
    [InlineData("Ștefan Țurcanu")]
    [InlineData("Ελένη Παπαδοπούλου")]
    [InlineData("Ana 😀 Pop")]
    [InlineData("Robert'); DROP TABLE taper_plans;--")]
    [InlineData("<img src=x onerror=alert(1)>")]
    [InlineData("  Leading Spaces")]
    public void StoreAPatientNameExactlyAsWritten(string patientName)
    {
        // execute

        _repository.Add(new NewTaperPlan(patientName, "STR", 40m, 4, new DateOnly(2026, 3, 1)));

        // verify

        _repository.List().Should().ContainSingle().Which.PatientName.Should().Be(patientName);
    }

    [Fact]
    public void ListPlansInTheOrderTheyWereSaved()
    {
        // setup

        foreach (var name in new[] { "First", "Second", "Third" })
            _repository.Add(new NewTaperPlan(name, "STR", 40m, 4, new DateOnly(2026, 3, 1)));

        // execute

        var plans = _repository.List();

        // verify

        plans.Select(plan => plan.PatientName).Should().Equal("First", "Second", "Third");
        plans.Select(plan => plan.ReferenceNumber).Should().Equal("TP-0001", "TP-0002", "TP-0003");
    }

    [Fact]
    public async Task SeedTheSamplePlansOnlyOnce()
    {
        // setup

        var connectionString = await postgres.CreateDatabaseAsync();
        await using (var firstRun = StartApi(connectionString))
            firstRun.CreateClient();

        // execute

        await using var secondRun = StartApi(connectionString);
        var plans = await secondRun.CreateClient().GetFromJsonAsync<List<PlanBody>>("/api/taper-plans");

        // verify

        plans.Should().ContainSingle(plan => plan.PatientName == "Maria Ionescu");
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
