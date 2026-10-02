namespace LoanLab.Application;

public sealed class LoanValidationException : Exception
{
    public LoanValidationException(string message) : base(message)
    {
    }
}
