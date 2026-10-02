namespace DoseLab.Application;

/// <summary>
/// Stands in for the network round trip to a real database.
/// Every repository and catalog call waits once.
/// </summary>
public sealed class SimulatedLatency(TimeSpan delay)
{
    public static readonly SimulatedLatency None = new(TimeSpan.Zero);

    public void Wait()
    {
        if (delay > TimeSpan.Zero)
            Thread.Sleep(delay);
    }
}
