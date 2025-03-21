namespace DimonSmart.StringDiff;

public class StringDiff(StringDiffOptions? options = null) : IStringDiff
{
    public StringDiffOptions? Options { get; } = options;

    public TextDiff ComputeDiff(string sourceText, string targetText)
    {
        // If no options or no tokenizer specified, use CharDiff for character-level diffing
        if (Options?.TokenBoundaryDetector == null)
        {
            return new CharDiff().ComputeDiff(sourceText, targetText);
        }

        // Use word-level diffing for SimpleTokenBoundaryDetector
        if (Options.TokenBoundaryDetector is SimpleTokenBoundaryDetector)
        {
            var wordDiff = new WordDiff(Options.TokenBoundaryDetector);
            var wordEdits = wordDiff.ComputeDiff(sourceText, targetText);
            var result = wordEdits.Select(e => e.ToStringEdit()).ToList();
            return new TextDiff(sourceText, targetText, result);
        }

        // Use WordDiff with custom tokenizer for all other cases
        var customWordDiff = new WordDiff(Options.TokenBoundaryDetector);
        var customEdits = customWordDiff.ComputeDiff(sourceText, targetText);
        var customResult = customEdits.Select(e => e.ToStringEdit()).ToList();
        return new TextDiff(sourceText, targetText, customResult);
    }
}
