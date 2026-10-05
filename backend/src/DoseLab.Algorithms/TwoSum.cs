namespace DoseLab.Algorithms;

/// <summary>
/// Finds two indices whose values add up to a target.
/// One pass with a dictionary of values already seen. Time O(n). Space O(n).
/// </summary>
public static class TwoSum
{
    /// <summary>
    /// Returns the two indices, earlier index first. Returns an empty array when no pair exists.
    /// Each index is used at most once.
    /// </summary>
    public static int[] IndicesThatSumTo(int[] values, int target)
    {
        ArgumentNullException.ThrowIfNull(values);

        var seen = new Dictionary<int, int>();

        for (var i = 0; i < values.Length; i++)
        {
            var need = target - values[i];
            if (seen.TryGetValue(need, out var earlier))
                return [earlier, i];

            seen[values[i]] = i;
        }

        return [];
    }
}
