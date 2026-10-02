namespace LoanLab.Application;

public sealed record LoanAccount(
    Guid Id,
    string CustomerName,
    decimal Amount,
    decimal AnnualInterestRate,
    int InstallmentCount,
    DateOnly CreatedOn);

public sealed record ScheduleLine(
    int Number,
    decimal Principal,
    decimal Interest,
    decimal Installment,
    decimal RemainingBalance);

public sealed record LoanSchedule(
    IReadOnlyList<ScheduleLine> Lines,
    decimal TotalPrincipal,
    decimal TotalInterest,
    decimal TotalInstallment);
