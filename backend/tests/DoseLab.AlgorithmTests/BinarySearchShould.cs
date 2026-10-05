using DoseLab.Algorithms;
using Xunit;

namespace DoseLab.AlgorithmTests;

public class BinarySearchShould
{
    [Fact]
    public void FindAValueInASortedArray()
    {
        // setup

        int[] sortedValues = [1, 3, 4, 7, 9, 12];

        // execute

        var index = BinarySearch.IndexOf(sortedValues, 7);

        // verify

        Assert.Equal(3, index);
    }

    [Fact]
    public void ReturnMinusOneWhenTheValueIsMissing()
    {
        // setup

        int[] sortedValues = [1, 3, 4, 7];

        // execute

        var index = BinarySearch.IndexOf(sortedValues, 5);

        // verify

        Assert.Equal(-1, index);
    }

    [Theory]
    [InlineData(1, 0)]
    [InlineData(12, 5)]
    public void FindTheFirstAndLastElement(int target, int expectedIndex)
    {
        // setup

        int[] sortedValues = [1, 3, 4, 7, 9, 12];

        // execute

        var index = BinarySearch.IndexOf(sortedValues, target);

        // verify

        Assert.Equal(expectedIndex, index);
    }
}
