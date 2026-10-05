using DoseLab.Algorithms;
using Xunit;

namespace DoseLab.AlgorithmTests;

public class TwoSumShould
{
    [Fact]
    public void ReturnTheIndicesOfThePairThatAddsUpToTheTarget()
    {
        // setup

        int[] values = [2, 7, 11, 15];

        // execute

        var indices = TwoSum.IndicesThatSumTo(values, 9);

        // verify

        Assert.Equal([0, 1], indices);
    }

    [Fact]
    public void UseEachIndexOnce()
    {
        // setup

        int[] values = [3, 3];

        // execute

        var indices = TwoSum.IndicesThatSumTo(values, 6);

        // verify

        Assert.Equal([0, 1], indices);
    }

    [Fact]
    public void ReturnAnEmptyArrayWhenNoPairExists()
    {
        // setup

        int[] values = [1, 2, 3];

        // execute

        var indices = TwoSum.IndicesThatSumTo(values, 10);

        // verify

        Assert.Empty(indices);
    }
}
