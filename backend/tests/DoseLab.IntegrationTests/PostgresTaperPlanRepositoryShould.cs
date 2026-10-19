using DoseLab.Application;
using DoseLab.IntegrationTests.Harness;
using DoseLab.Postgres;
using FluentAssertions;
using Npgsql;
using Xunit;

namespace DoseLab.IntegrationTests;

// Lesson 12: the repository against a real PostgreSQL in Docker. Exercises: git grep "TODO(L12"
// Needs Docker. Without Docker, skip these tests with: dotnet test --filter "Category!=Docker"
[Collection(PostgresCollection.Name)]
[Trait("Category", "Docker")]
public class PostgresTaperPlanRepositoryShould(PostgresFixture postgres) : IAsyncLifetime
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

    [Fact]
    public void ReadBackASavedPlanWithTheSameValues()
    {
        // setup

        var draft = new NewTaperPlan("Ana Pop", "CLM", 10m, 3, new DateOnly(2026, 3, 1));

        // execute

        var saved = _repository.Add(draft);

        // verify

        var read = _repository.List().Should().ContainSingle().Subject;
        read.Should().Be(saved);
        read.ReferenceNumber.Should().Be("TP-0001");
        read.CreatedOn.Should().Be(new DateOnly(2026, 3, 1));
    }

    [Fact]
    public void GiveEveryPlanAUniqueReferenceUnderParallelSaves()
    {
        // setup

        var draft = new NewTaperPlan("Parallel", "STR", 40m, 4, new DateOnly(2026, 3, 1));

        // execute

        Parallel.For(0, 50, _ => _repository.Add(draft));

        // verify

        _repository.List().Select(plan => plan.ReferenceNumber).Should().HaveCount(50).And.OnlyHaveUniqueItems();
    }

    [Theory(Skip = "TODO(L12-1)")]
    [InlineData("6.67")]
    public void KeepTheStartingDoseExactly(string dose)
    {
        // setup

        // TODO(L12-1): add more doses that could lose precision in a database column:
        // many decimals (12.345), the maximum (80), the minimum (1), a value like 0.1.
        var startingDailyDoseMg = decimal.Parse(dose, System.Globalization.CultureInfo.InvariantCulture);

        // execute

        _repository.Add(new NewTaperPlan("Precise", "STR", startingDailyDoseMg, 4, new DateOnly(2026, 3, 1)));

        // verify

        // TODO(L12-1): read the plan back with _repository.List() and assert the dose is exactly the same.
        Assert.Fail("Not written yet.");
    }

    [Theory(Skip = "TODO(L12-2)")]
    [InlineData("Ana Pop")]
    public void StoreAPatientNameExactlyAsWritten(string patientName)
    {
        // TODO(L12-2): error guessing. Add names that often break code that talks to a database or a screen:
        // quotes, Romanian letters, other alphabets, emoji, text that looks like SQL or HTML, leading spaces.
        // Which layer would trim the leading spaces: the repository or the service?

        // execute

        _repository.Add(new NewTaperPlan(patientName, "STR", 40m, 4, new DateOnly(2026, 3, 1)));

        // verify

        // TODO(L12-2): read it back with _repository.List() and assert the name is exactly the same.
        Assert.Fail("Not written yet.");
    }

    [Fact(Skip = "TODO(L12-3)")]
    public void ListPlansInTheOrderTheyWereSaved()
    {
        // TODO(L12-3): save three plans named "First", "Second", "Third". Assert List() returns them in that order,
        // with references TP-0001, TP-0002, TP-0003. Hint: .Should().Equal(...) or .ContainInOrder(...).
        Assert.Fail("Not written yet.");
    }
}
