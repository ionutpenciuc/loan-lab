namespace DoseLab.Application;

public sealed class InMemoryMedicationCatalog : IMedicationCatalog
{
    private static readonly IReadOnlyList<Medication> Medications = new[]
    {
        new Medication("STR", "Steriva", 80m),
        new Medication("CLM", "Calmafen", 40m),
        new Medication("NRV", "Nervalin", 20m),
    };

    private readonly SimulatedLatency _latency;

    public InMemoryMedicationCatalog(SimulatedLatency latency)
    {
        _latency = latency;
    }

    public IReadOnlyList<Medication> List()
    {
        _latency.Wait();
        return Medications;
    }

    public Medication? Find(string code)
    {
        _latency.Wait();
        return Medications.FirstOrDefault(medication => medication.Code == code);
    }
}
