using DoseLab.Algorithms;
using Xunit;

namespace DoseLab.AlgorithmTests;

public class BracketMatcherShould
{
    [Theory]
    [InlineData("")]
    [InlineData("()")]
    [InlineData("([]){}")]
    [InlineData("{[()()]}")]
    public void AcceptABalancedString(string text)
    {
        // execute

        var balanced = BracketMatcher.IsBalanced(text);

        // verify

        Assert.True(balanced);
    }

    [Theory]
    [InlineData(")")]
    [InlineData("([)]")]
    [InlineData("(()")]
    [InlineData("(a)")]
    public void RejectAnUnbalancedString(string text)
    {
        // execute

        var balanced = BracketMatcher.IsBalanced(text);

        // verify

        Assert.False(balanced);
    }
}
