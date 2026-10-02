using DoseLab.Application;

namespace DoseLab.Api.Contracts;

public sealed class PreviewScheduleRequest
{
    public string? MedicationCode { get; set; }
    public decimal StartingDailyDoseMg { get; set; }
    public int WeekCount { get; set; }
}

public sealed class CreateTaperPlanRequest
{
    public string? PatientName { get; set; }
    public string? MedicationCode { get; set; }
    public decimal StartingDailyDoseMg { get; set; }
    public int WeekCount { get; set; }
}

public sealed record ErrorResponse(string Message);

public sealed record MedicationResponse(string Code, string Name, decimal MaxDailyDoseMg)
{
    public static MedicationResponse From(Medication medication) =>
        new(medication.Code, medication.Name, medication.MaxDailyDoseMg);
}

public sealed record TaperPlanResponse(
    Guid Id,
    string ReferenceNumber,
    string PatientName,
    string MedicationCode,
    string MedicationName,
    decimal StartingDailyDoseMg,
    int WeekCount,
    DateOnly CreatedOn)
{
    public static TaperPlanResponse From(TaperPlanSummary summary) =>
        new(
            summary.Plan.Id,
            summary.Plan.ReferenceNumber,
            summary.Plan.PatientName,
            summary.Plan.MedicationCode,
            summary.MedicationName,
            summary.Plan.StartingDailyDoseMg,
            summary.Plan.WeekCount,
            summary.Plan.CreatedOn);
}

public sealed record ScheduleWeekResponse(
    int Week,
    decimal DailyDoseMg,
    decimal WeeklyTotalMg,
    decimal CumulativeTotalMg);

public sealed record ScheduleResponse(IReadOnlyList<ScheduleWeekResponse> Weeks, decimal TotalMg)
{
    public static ScheduleResponse From(TaperSchedule schedule) =>
        new(
            schedule.Weeks.Select(week => new ScheduleWeekResponse(
                week.Week,
                week.DailyDoseMg,
                week.WeeklyTotalMg,
                week.CumulativeTotalMg)).ToList(),
            schedule.TotalMg);
}
