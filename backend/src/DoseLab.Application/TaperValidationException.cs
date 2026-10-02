namespace DoseLab.Application;

public sealed class TaperValidationException : Exception
{
    public TaperValidationException(string message) : base(message)
    {
    }
}
