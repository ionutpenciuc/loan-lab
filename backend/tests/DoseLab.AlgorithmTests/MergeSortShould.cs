using DoseLab.Algorithms;
using Xunit;

namespace DoseLab.AlgorithmTests;

public class MergeSortShould
{
    [Fact]
    public void OrderAnUnsortedArray()
    {
        // setup

        int[] values = [5, 1, 4, 2, 8, 1];

        // execute

        var sorted = MergeSort.Sort(values);

        // verify

        Assert.Equal([1, 1, 2, 4, 5, 8], sorted);
        Assert.Equal([5, 1, 4, 2, 8, 1], values);
    }

    [Fact]
    public void KeepAnEmptyArrayEmpty()
    {
        // setup

        int[] values = [];

        // execute

        var sorted = MergeSort.Sort(values);

        // verify

        Assert.Empty(sorted);
    }
}
