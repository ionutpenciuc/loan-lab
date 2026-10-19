using DoseLab.Application;
using DoseLab.UnitTests.Builders;
using FluentAssertions;
using Microsoft.Extensions.Time.Testing;
using NSubstitute;
using Xunit;

namespace DoseLab.UnitTests;

// Lesson 11: mocks (NSubstitute), a fake clock (FakeTimeProvider), a test data builder, and FluentAssertions.
// The first two tests are samples. The skipped ones are your exercises: git grep "TODO(L11"
public class TaperPlanServiceWithMocksShould
{
    private readonly ITaperPlanRepository _repository = Substitute.For<ITaperPlanRepository>();
    private readonly IMedicationCatalog _catalog = Substitute.For<IMedicationCatalog>();
    private readonly FakeTimeProvider _clock = new(new DateTimeOffset(2026, 3, 1, 9, 0, 0, TimeSpan.Zero));
    private readonly TaperPlanService _service;

    public TaperPlanServiceWithMocksShould()
    {
        var calmafen = new Medication("CLM", "Calmafen", 40m);
        _catalog.Find("CLM").Returns(calmafen);
        _catalog.List().Returns([calmafen]);
        _repository.Add(Arg.Any<NewTaperPlan>()).Returns(call => ToPlan(call.Arg<NewTaperPlan>()));

        _service = new TaperPlanService(_repository, _catalog, _clock);
    }

    [Fact]
    public void NotSaveAPlanForAnUnknownMedication()
    {
        // setup

        _catalog.Find("XXX").Returns((Medication?)null);

        // execute

        var act = () => _service.Create("Ana Pop", "XXX", 10m, 3);

        // verify

        act.Should().Throw<TaperValidationException>().WithMessage("Unknown medication.");
        _repository.DidNotReceive().Add(Arg.Any<NewTaperPlan>());
    }

    [Fact]
    public void StampANewPlanWithTheDateFromTheClock()
    {
        // execute

        _service.Create("Ana Pop", "CLM", 10m, 3);

        // verify

        _repository.Received(1).Add(Arg.Is<NewTaperPlan>(draft =>
            draft.PatientName == "Ana Pop" && draft.CreatedOn == new DateOnly(2026, 3, 1)));
    }

    [Fact(Skip = "TODO(L11-1)")]
    public void ReadTheMedicationCatalogOnceWhenListingPlans()
    {
        // setup

        // TODO(L11-1): make the repository return 200 plans for Calmafen. Use TaperPlanBuilder:
        // _repository.List().Returns(TaperPlanBuilder.APlan().WithMedication("CLM").BuildMany(200));

        // execute

        var plans = _service.List();

        // verify

        plans.Should().HaveCount(200);
        plans.Should().OnlyContain(summary => summary.MedicationName == "Calmafen");
        // TODO(L11-1): assert the catalog was read once in total, and never once per plan:
        // _catalog.Received(1).List(); and _catalog.DidNotReceive().Find(Arg.Any<string>());
    }

    [Fact(Skip = "TODO(L11-2)")]
    public void NotTouchTheRepositoryWhenPreviewing()
    {
        // execute

        // TODO(L11-2): preview Calmafen, 10 mg over 3 weeks.

        // verify

        // TODO(L11-2): no call of any kind reached the repository. Hint: _repository.ReceivedCalls().Should()...
        Assert.Fail("Not written yet.");
    }

    [Theory(Skip = "TODO(L11-3)")]
    [InlineData(1, 23, 59, "2026-03-01")]
    [InlineData(2, 0, 0, "2026-03-02")]
    public void UseTheUtcDateForCreatedOn(int dayOfMarch, int hour, int minute, string expectedDate)
    {
        // setup

        // TODO(L11-3): one case fails. The clock below is set to Bucharest time (UTC+2), but the cases are UTC.
        // (A FakeTimeProvider only moves forward. It starts at 09:00 UTC on 1 March: see the class fields.)
        // Fix the setup, not the expected dates. Then think: a nurse in Bucharest saves at 01:00 local time on 2 March.
        // Which date is stored? Is that right? Write the question for the product owner in notes/questions.md.
        _clock.SetUtcNow(new DateTimeOffset(2026, 3, dayOfMarch, hour, minute, 0, TimeSpan.FromHours(2)));

        // execute

        var created = _service.Create("Ana Pop", "CLM", 10m, 3);

        // verify

        created.Plan.CreatedOn.Should().Be(DateOnly.Parse(expectedDate));
    }

    [Fact(Skip = "TODO(L11-4)")]
    public void ReportTheSameErrorWithFluentAssertionsAsWithAssert()
    {
        // setup

        var act = () => _service.Preview("CLM", 40.01m, 3);

        // execute & verify

        // TODO(L11-4): rewrite this xUnit assert with FluentAssertions: act.Should().Throw<...>().WithMessage(...).
        // Then break the expected message on purpose and compare the two failure messages. Which helps more?
        var exception = Assert.Throws<TaperValidationException>(act);
        Assert.Equal("Starting daily dose must not exceed 40 mg for Calmafen.", exception.Message);
    }

    private static TaperPlan ToPlan(NewTaperPlan draft) =>
        new(Guid.NewGuid(), "TP-0001", draft.PatientName, draft.MedicationCode,
            draft.StartingDailyDoseMg, draft.WeekCount, draft.CreatedOn);
}
