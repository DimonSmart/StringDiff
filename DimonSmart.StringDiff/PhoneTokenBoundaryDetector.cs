using System.Text.RegularExpressions;

namespace DimonSmart.StringDiff;
public class PhoneTokenBoundaryDetector : ITokenBoundaryDetector
{
    // A basic regex for phone numbers. Adjust as needed.
    private static readonly Regex PhoneRegex = new Regex(@"\+?\d[\d\s\-]{7,}\d", RegexOptions.Compiled);

    public void TokenizeSpan(ReadOnlySpan<char> text, Span<Range> tokenRanges, out int tokenCount)
    {
        tokenCount = 0;
        var input = text.ToString();
        var matches = PhoneRegex.Matches(input);
        foreach (Match match in matches)
        {
            tokenRanges[tokenCount++] = new Range(match.Index, match.Index + match.Length);
        }
    }
}