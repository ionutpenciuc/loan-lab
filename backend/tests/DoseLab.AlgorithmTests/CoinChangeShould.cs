using DoseLab.Algorithms;
using Xunit;

namespace DoseLab.AlgorithmTests;

public class CoinChangeShould
{
    [Fact]
    public void UseTheFewestCoinsThatMakeTheAmount()
    {
        // setup

        int[] coins = [1, 5, 10];

        // execute

        var count = CoinChange.MinimumCoins(coins, 18);

        // verify

        Assert.Equal(5, count);
    }

    [Fact]
    public void ReturnMinusOneWhenTheAmountCannotBeMade()
    {
        // setup

        int[] coins = [2];

        // execute

        var count = CoinChange.MinimumCoins(coins, 3);

        // verify

        Assert.Equal(-1, count);
    }

    [Fact]
    public void ReturnZeroForAZeroAmount()
    {
        // setup

        int[] coins = [1, 5];

        // execute

        var count = CoinChange.MinimumCoins(coins, 0);

        // verify

        Assert.Equal(0, count);
    }
}
