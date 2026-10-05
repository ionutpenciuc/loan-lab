namespace DoseLab.Algorithms;

/// <summary>
/// Finds a value in a sorted array by cutting the search range in half each step.
/// Time O(log n). Space O(1). The array must be sorted in ascending order.
/// </summary>
public static class BinarySearch
{
    /// <summary>
    /// Returns the index of <paramref name="target"/>, or -1 when it is missing.
    /// </summary>
    public static int IndexOf(int[] sortedValues, int target)
    {
        ArgumentNullException.ThrowIfNull(sortedValues);

        var low = 0;
        var high = sortedValues.Length - 1;

        while (low <= high)
        {
            var mid = low + (high - low) / 2;
            var value = sortedValues[mid];

            if (value == target)
                return mid;

            if (value < target)
                low = mid + 1;
            else
                high = mid - 1;
        }

        return -1;
    }
}
