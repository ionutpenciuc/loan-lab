namespace DoseLab.Algorithms;

/// <summary>
/// Largest sum of any contiguous slice of the array (Kadane's algorithm).
/// At each index, either extend the current slice or start a new one.
/// Time O(n). Space O(1).
/// </summary>
public static class MaxSubarray
{
    public static int LargestSum(int[] values)
    {
        ArgumentNullException.ThrowIfNull(values);
        if (values.Length == 0)
            throw new ArgumentException("Need at least one value.", nameof(values));

        var best = values[0];
        var current = values[0];

        for (var i = 1; i < values.Length; i++)
        {
            current = Math.Max(values[i], current + values[i]);
            best = Math.Max(best, current);
        }

        return best;
    }
}
