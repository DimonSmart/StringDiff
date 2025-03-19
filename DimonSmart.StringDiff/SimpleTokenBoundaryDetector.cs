namespace DimonSmart.StringDiff;

public class SimpleTokenBoundaryDetector : ITokenBoundaryDetector
{
    public static SimpleTokenBoundaryDetector Instance = new();

    public void TokenizeSpan(ReadOnlySpan<char> text, Span<Range> tokenRanges, out int tokenCount)
    {
        tokenCount = 0;
        if (text.IsEmpty) return;

        var start = 0;
        var inWord = char.IsLetterOrDigit(text[0]);

        for (var i = 1; i < text.Length; i++)
        {
            var currentIsWord = char.IsLetterOrDigit(text[i]);
            if (currentIsWord != inWord)
            {
                tokenRanges[tokenCount++] = new Range(start, i);
                start = i;
                inWord = currentIsWord;
            }
        }

        // Don't forget the last token
        if (start < text.Length)
        {
            tokenRanges[tokenCount++] = new Range(start, text.Length);
        }
    }
}