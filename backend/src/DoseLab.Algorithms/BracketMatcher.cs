namespace DoseLab.Algorithms;

/// <summary>
/// Checks that (), [], and {} are opened and closed in the right order.
/// A stack holds the open brackets. Time O(n). Space O(n).
/// </summary>
public static class BracketMatcher
{
    public static bool IsBalanced(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        var open = new Stack<char>();

        foreach (var symbol in text)
        {
            if (symbol is '(' or '[' or '{')
            {
                open.Push(symbol);
                continue;
            }

            if (symbol is not (')' or ']' or '}'))
                return false;

            if (open.Count == 0)
                return false;

            if (!Matches(open.Pop(), symbol))
                return false;
        }

        return open.Count == 0;
    }

    private static bool Matches(char opened, char closed) =>
        (opened, closed) is ('(', ')') or ('[', ']') or ('{', '}');
}
