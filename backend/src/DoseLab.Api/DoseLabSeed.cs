using DoseLab.Application;

namespace DoseLab.Api;

public static class DoseLabSeed
{
    public static void Apply(ITaperPlanRepository repository)
    {
        repository.Add(new NewTaperPlan("Maria Ionescu", "STR", 40m, 4, new DateOnly(2026, 1, 15)));
        repository.Add(new NewTaperPlan("Andrei Stan", "CLM", 10m, 3, new DateOnly(2026, 2, 1)));
    }
}
