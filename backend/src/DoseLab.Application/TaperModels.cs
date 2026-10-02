namespace DoseLab.Application;

public sealed record Medication(string Code, string Name, decimal MaxDailyDoseMg);

public sealed record NewTaperPlan(
    string PatientName,
    string MedicationCode,
    decimal StartingDailyDoseMg,
    int WeekCount,
    DateOnly CreatedOn);

public sealed record TaperPlan(
    Guid Id,
    string ReferenceNumber,
    string PatientName,
    string MedicationCode,
    decimal StartingDailyDoseMg,
    int WeekCount,
    DateOnly CreatedOn);

public sealed record TaperPlanSummary(TaperPlan Plan, string MedicationName);

public sealed record ScheduleWeek(
    int Week,
    decimal DailyDoseMg,
    decimal WeeklyTotalMg,
    decimal CumulativeTotalMg);

public sealed record TaperSchedule(IReadOnlyList<ScheduleWeek> Weeks, decimal TotalMg);
