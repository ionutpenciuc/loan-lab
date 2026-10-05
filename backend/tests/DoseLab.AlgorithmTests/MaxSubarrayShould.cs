using DoseLab.Algorithms;
using Xunit;

namespace DoseLab.AlgorithmTests;

public class MaxSubarrayShould
{
    [Fact]
    public void ReturnTheLargestContiguousSum()
    {
        // setup

        int[] values = [-2, 1, -3, 4, -1, 2, 1, -5, 4];

        // execute

        var sum = MaxSubarray.LargestSum(values);

        // verify

        Assert.Equal(6, sum);
    }

    [Fact]
    public void ReturnTheLargestValueWhenEveryValueIsNegative()
    {
        // setup

        int[] values = [-8, -3, -6];

        // execute

        var sum = MaxSubarray.LargestSum(values);

        // verify

        Assert.Equal(-3, sum);
    }
}
