namespace DimonSmart.StringDiff;

public interface ITokenBoundaryDetector
{
    IEnumerable<string> Tokenize(string text);
    
    // New Span-based method that avoids allocations
    void TokenizeSpan(ReadOnlySpan<char> text, Span<Range> tokenRanges, out int tokenCount);
}
