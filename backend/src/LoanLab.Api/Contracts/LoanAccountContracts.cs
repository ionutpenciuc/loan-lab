using LoanLab.Application;

namespace LoanLab.Api.Contracts;

public sealed class PreviewScheduleRequest
{
    public decimal Amount { get; set; }
    public decimal AnnualInterestRate { get; set; }
    public int InstallmentCount { get; set; }
}

public sealed class CreateLoanAccountRequest
{
    public string? CustomerName { get; set; }
    public decimal Amount { get; set; }
    public decimal AnnualInterestRate { get; set; }
    public int InstallmentCount { get; set; }
}

public sealed record ErrorResponse(string Message);

public sealed record LoanAccountResponse(
    Guid Id,
    string CustomerName,
    decimal Amount,
    decimal AnnualInterestRate,
    int InstallmentCount,
    DateOnly CreatedOn)
{
    public static LoanAccountResponse From(LoanAccount account) =>
        new(account.Id, account.CustomerName, account.Amount, account.AnnualInterestRate, account.InstallmentCount, account.CreatedOn);
}

public sealed record ScheduleLineResponse(
    int Number,
    decimal Principal,
    decimal Interest,
    decimal Installment,
    decimal RemainingBalance);

public sealed record ScheduleResponse(
    IReadOnlyList<ScheduleLineResponse> Lines,
    decimal TotalPrincipal,
    decimal TotalInterest,
    decimal TotalInstallment)
{
    public static ScheduleResponse From(LoanSchedule schedule) =>
        new(
            schedule.Lines.Select(line => new ScheduleLineResponse(
                line.Number,
                line.Principal,
                line.Interest,
                line.Installment,
                line.RemainingBalance)).ToList(),
            schedule.TotalPrincipal,
            schedule.TotalInterest,
            schedule.TotalInstallment);
}
