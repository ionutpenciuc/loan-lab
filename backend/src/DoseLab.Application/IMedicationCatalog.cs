namespace DoseLab.Application;

public interface IMedicationCatalog
{
    IReadOnlyList<Medication> List();

    Medication? Find(string code);
}
