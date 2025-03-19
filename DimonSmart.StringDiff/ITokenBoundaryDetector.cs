namespace DimonSmart.StringDiff;

public interface ITokenBoundaryDetector
{
    void TokenizeSpan(ReadOnlySpan<char> text, Span<Range> tokenRanges, out int tokenCount);
}
