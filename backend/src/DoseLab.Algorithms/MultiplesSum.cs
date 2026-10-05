namespace DoseLab.Algorithms;

/// <summary>
/// Sum of the positive multiples of 3 or 5 or 7 that are strictly below n.
/// A number that matches more than one of these is added once.
/// Time O(n). Space O(1). Constraint: 0 ≤ n &lt; 1000.
/// </summary>
public static class MultiplesSum
{
    public static int ComputeMultiplesSum(int n)
    {
        if (n < 0 || n >= 1000)
            throw new ArgumentOutOfRangeException(nameof(n));

        var sum = 0;

        for (var value = 1; value < n; value++)
        {
            if (value % 3 == 0 || value % 5 == 0 || value % 7 == 0)
                sum += value;
        }

        return sum;
    }
}
