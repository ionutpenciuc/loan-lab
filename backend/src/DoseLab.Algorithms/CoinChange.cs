namespace DoseLab.Algorithms;

/// <summary>
/// Fewest coins that add up to an amount. Each coin value may be used many times.
/// Dynamic programming: best[sum] is the fewest coins that make that sum.
/// Time O(amount × coins). Space O(amount). Returns -1 when the amount cannot be made.
/// </summary>
public static class CoinChange
{
    public static int MinimumCoins(int[] coins, int amount)
    {
        ArgumentNullException.ThrowIfNull(coins);
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount));

        foreach (var coin in coins)
        {
            if (coin <= 0)
                throw new ArgumentOutOfRangeException(nameof(coins));
        }

        var impossible = amount + 1;
        var best = new int[amount + 1];
        Array.Fill(best, impossible);
        best[0] = 0;

        for (var sum = 1; sum <= amount; sum++)
        {
            foreach (var coin in coins)
            {
                if (coin <= sum)
                    best[sum] = Math.Min(best[sum], best[sum - coin] + 1);
            }
        }

        return best[amount] > amount ? -1 : best[amount];
    }
}
