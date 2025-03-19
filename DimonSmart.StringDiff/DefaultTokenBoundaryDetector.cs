namespace DimonSmart.StringDiff;

public class DefaultTokenBoundaryDetector : ITokenBoundaryDetector
{
    public static DefaultTokenBoundaryDetector Instance = new();
    
    public IEnumerable<string> Tokenize(string text)
    {
        var result = new List<string>();
        if (string.IsNullOrEmpty(text))
            return result;

        Span<Range> ranges = stackalloc Range[text.Length];
        TokenizeSpan(text.AsSpan(), ranges, out int count);

        for (int i = 0; i < count; i++)
        {
            result.Add(text[ranges[i]].ToString());
        }

        return result;
    }

    public void TokenizeSpan(ReadOnlySpan<char> text, Span<Range> tokenRanges, out int tokenCount)
    {
        tokenCount = 0;
        if (text.IsEmpty) return;

        int start = 0;
        bool inWord = char.IsLetterOrDigit(text[0]);

        for (int i = 1; i < text.Length; i++)
        {
            bool currentIsWord = char.IsLetterOrDigit(text[i]);
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