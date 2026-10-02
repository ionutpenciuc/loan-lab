using DoseLab.Application;
using Xunit;

namespace DoseLab.UnitTests;

public class TaperPlanServiceShould
{
    [Fact]
    public void SaveAPlanThatAppearsInTheList()
    {
        // setup

        var repository = new InMemoryTaperPlanRepository(SimulatedLatency.None);
        var catalog = new InMemoryMedicationCatalog(SimulatedLatency.None);
        var service = new TaperPlanService(repository, catalog, TimeProvider.System);

        // execute

        service.Create("Ana Pop", "STR", 40m, 4);

        // verify

        var saved = Assert.Single(service.List());
        Assert.Equal("Ana Pop", saved.Plan.PatientName);
        Assert.Equal("STR", saved.Plan.MedicationCode);
        Assert.Equal("Steriva", saved.MedicationName);
        Assert.Equal(40m, saved.Plan.StartingDailyDoseMg);
        Assert.Equal(4, saved.Plan.WeekCount);
        Assert.Equal("TP-0001", saved.Plan.ReferenceNumber);
    }

    // Add your own [Fact] or [Theory] methods below this line.
}
