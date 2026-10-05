using DoseLab.Algorithms;
using Xunit;

namespace DoseLab.AlgorithmTests;

public class MultiplesSumShould
{
    [Fact]
    public void SumThePositiveMultiplesStrictlyBelowTwelve()
    {
        // setup

        var n = 12;

        // execute

        var sum = MultiplesSum.ComputeMultiplesSum(n);

        // verify

        Assert.Equal(40, sum);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(1)]
    [InlineData(3)]
    public void ReturnZeroWhenNoPositiveMultipleIsStrictlyBelowN(int n)
    {
        // execute

        var sum = MultiplesSum.ComputeMultiplesSum(n);

        // verify

        Assert.Equal(0, sum);
    }

    [Fact]
    public void AddASharedMultipleOnce()
    {
        // setup

        var n = 16;

        // execute

        var sum = MultiplesSum.ComputeMultiplesSum(n);

        // verify

        Assert.Equal(81, sum);
    }
}
