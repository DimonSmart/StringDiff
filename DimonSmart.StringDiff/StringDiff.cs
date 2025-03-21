namespace DimonSmart.StringDiff;

public class StringDiff(StringDiffOptions? options = null) : IStringDiff
{
    public StringDiffOptions? Options { get; } = options;

    public TextDiff ComputeDiff(string sourceText, string targetText)
    {
        if (Options?.TokenBoundaryDetector is SimpleTokenBoundaryDetector)
        {
            // Use word-level diffing for SimpleTokenBoundaryDetector
            var wordDiff = new WordDiff(Options.TokenBoundaryDetector);
            var wordEdits = wordDiff.ComputeDiff(sourceText, targetText);
            var result = wordEdits.Select(e => e.ToStringEdit()).ToList();
            return new TextDiff(sourceText, targetText, result);
        }
        else
        {
            // Use character-level diffing for other cases
            var charDiff = new CharDiff(Options?.TokenBoundaryDetector ?? new CharacterTokenBoundaryDetector());
            var charEdits = charDiff.ComputeDiff(sourceText, targetText);
            var result = charEdits.Select(e => e.ToStringEdit()).ToList();
            return new TextDiff(sourceText, targetText, result);
        }
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
