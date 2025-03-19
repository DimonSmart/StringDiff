using DimonSmart.StringDiff;

namespace DimonSmart.StringDiffTests;

public static class TokenizationHelper
{
    public static IEnumerable<string> TokenizeToStrings(this ITokenBoundaryDetector detector, string text)
    {
        if (string.IsNullOrEmpty(text))
            return Array.Empty<string>();

        Span<Range> ranges = stackalloc Range[text.Length];
        detector.TokenizeSpan(text.AsSpan(), ranges, out var count);

        var result = new string[count];
        for (var i = 0; i < count; i++)
        {
            result[i] = text[ranges[i]].ToString();
        }

        return result;
    }
}