// Answer key for lesson 11 (unit level). Copy into backend/tests/DoseLab.UnitTests/ to run it.
// ReadTheMedicationCatalogOnceWhenListingPlans also catches bug 07 at the unit level.
using DoseLab.Application;
using DoseLab.UnitTests.Builders;
using FluentAssertions;
using Microsoft.Extensions.Time.Testing;
using NSubstitute;
using Xunit;

namespace DoseLab.UnitTests;

public class TaperPlanServiceWithMocksSolutionShould
{
    private readonly ITaperPlanRepository _repository = Substitute.For<ITaperPlanRepository>();
    private readonly IMedicationCatalog _catalog = Substitute.For<IMedicationCatalog>();
    private readonly FakeTimeProvider _clock = new(new DateTimeOffset(2026, 3, 1, 9, 0, 0, TimeSpan.Zero));
    private readonly TaperPlanService _service;

    public TaperPlanServiceWithMocksSolutionShould()
    {
        var calmafen = new Medication("CLM", "Calmafen", 40m);
        _catalog.Find("CLM").Returns(calmafen);
        _catalog.List().Returns([calmafen]);
        _repository.Add(Arg.Any<NewTaperPlan>()).Returns(call => ToPlan(call.Arg<NewTaperPlan>()));

        _service = new TaperPlanService(_repository, _catalog, _clock);
    }

    [Fact]
    public void ReadTheMedicationCatalogOnceWhenListingPlans()
    {
        // setup

        _repository.List().Returns(TaperPlanBuilder.APlan().WithMedication("CLM").BuildMany(200));

        // execute

        var plans = _service.List();

        // verify

        plans.Should().HaveCount(200);
        plans.Should().OnlyContain(summary => summary.MedicationName == "Calmafen");
        _catalog.Received(1).List();
        _catalog.DidNotReceive().Find(Arg.Any<string>());
    }

    [Fact]
    public void NotTouchTheRepositoryWhenPreviewing()
    {
        // execute

        _service.Preview("CLM", 10m, 3);

        // verify

        _repository.ReceivedCalls().Should().BeEmpty();
    }

    [Theory]
    [InlineData(1, 23, 59, "2026-03-01")]
    [InlineData(2, 0, 0, "2026-03-02")]
    public void UseTheUtcDateForCreatedOn(int dayOfMarch, int hour, int minute, string expectedDate)
    {
        // setup

        _clock.SetUtcNow(new DateTimeOffset(2026, 3, dayOfMarch, hour, minute, 0, TimeSpan.Zero));

        // execute

        var created = _service.Create("Ana Pop", "CLM", 10m, 3);

        // verify

        created.Plan.CreatedOn.Should().Be(DateOnly.Parse(expectedDate));
    }

    [Fact]
    public void ReportTheSameErrorWithFluentAssertionsAsWithAssert()
    {
        // setup

        var act = () => _service.Preview("CLM", 40.01m, 3);

        // execute & verify

        act.Should().Throw<TaperValidationException>()
            .WithMessage("Starting daily dose must not exceed 40 mg for Calmafen.");
    }

    private static TaperPlan ToPlan(NewTaperPlan draft) =>
        new(Guid.NewGuid(), "TP-0001", draft.PatientName, draft.MedicationCode,
            draft.StartingDailyDoseMg, draft.WeekCount, draft.CreatedOn);
}
