namespace DoseLab.Application;

/// <summary>
/// Stands in for the network round trip to a real database.
/// Every repository and catalog call waits once.
/// </summary>
public sealed class SimulatedLatency
{
    public static readonly SimulatedLatency None = new(TimeSpan.Zero);

    private readonly TimeSpan _delay;

    public SimulatedLatency(TimeSpan delay)
    {
        _delay = delay;
    }

    public void Wait()
    {
        if (_delay > TimeSpan.Zero)
            Thread.Sleep(_delay);
    }
}
