namespace DoseLab.Application;

public interface ITaperPlanRepository
{
    IReadOnlyList<TaperPlan> List();

    TaperPlan Add(NewTaperPlan draft);
}
