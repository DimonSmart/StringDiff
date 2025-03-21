namespace DimonSmart.StringDiff;

public interface ITokenBoundaryDetector
{
    /// <summary>
    /// Populates tokenRanges with tokens found in the text.
    /// </summary>
    void TokenizeSpan(ReadOnlySpan<char> text, Span<Range> tokenRanges, out int tokenCount);
}
