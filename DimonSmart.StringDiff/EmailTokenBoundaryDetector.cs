using System.Text.RegularExpressions;

namespace DimonSmart.StringDiff;

public class EmailTokenBoundaryDetector : ITokenBoundaryDetector
{
    private static readonly Regex EmailRegex = new Regex(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}", RegexOptions.Compiled);

    public void TokenizeSpan(ReadOnlySpan<char> text, Span<Range> tokenRanges, out int tokenCount)
    {
        tokenCount = 0;
        var matches = EmailRegex.Matches(text.ToString());
        foreach (Match match in matches)
        {
            tokenRanges[tokenCount++] = new Range(match.Index, match.Index + match.Length);
        }
    }
}
