namespace DimonSmart.StringDiff;

public class StringDiff(StringDiffOptions? options = null) : IStringDiff
{
    public StringDiffOptions? Options { get; } = options;

    public TextDiff ComputeDiff(string sourceText, string targetText)
    {
        var genericDiff = new GenericDiff<string>(Options?.TokenBoundaryDetector ?? new CharacterTokenBoundaryDetector(), null);
        var genericEdits = genericDiff.ComputeDiff(sourceText, targetText);
        var result = genericEdits.Select(e => e.ToStringEdit()).ToList();
        return new TextDiff(sourceText, targetText, result);
    }

    private class CharacterTokenBoundaryDetector : ITokenBoundaryDetector
    {
        public void TokenizeSpan(ReadOnlySpan<char> text, Span<Range> tokenRanges, out int tokenCount)
        {
            tokenCount = text.Length;
            for (var i = 0; i < tokenCount; i++)
            {
                tokenRanges[i] = new Range(i, i + 1);
            }
        }
    }
}
