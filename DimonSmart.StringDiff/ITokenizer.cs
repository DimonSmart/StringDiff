namespace DimonSmart.StringDiff;

public interface ITokenizer
{
    /// <summary>
    /// Splits the entire text into a sequence of tokens.
    /// </summary>
    void TokenizeSpan(ReadOnlySpan<char> text, Span<Range> tokenRanges, out int tokenCount);
}