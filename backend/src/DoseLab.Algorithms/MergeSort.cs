namespace DoseLab.Algorithms;

/// <summary>
/// Sorts by splitting the array in half, sorting each half, then merging the halves.
/// Stable. Time O(n log n). Space O(n). The input array is not changed.
/// </summary>
public static class MergeSort
{
    public static int[] Sort(int[] values)
    {
        ArgumentNullException.ThrowIfNull(values);

        var copy = values.ToArray();
        SortRange(copy, 0, copy.Length);
        return copy;
    }

    private static void SortRange(int[] values, int start, int end)
    {
        var length = end - start;
        if (length <= 1)
            return;

        var mid = start + length / 2;
        SortRange(values, start, mid);
        SortRange(values, mid, end);
        Merge(values, start, mid, end);
    }

    private static void Merge(int[] values, int start, int mid, int end)
    {
        var left = values[start..mid];
        var right = values[mid..end];
        var i = 0;
        var j = 0;
        var k = start;

        while (i < left.Length && j < right.Length)
        {
            if (left[i] <= right[j])
                values[k++] = left[i++];
            else
                values[k++] = right[j++];
        }

        while (i < left.Length)
            values[k++] = left[i++];

        while (j < right.Length)
            values[k++] = right[j++];
    }
}
