namespace DimonSmart.StringDiff;

public class StringDiff(StringDiffOptions? options = null) : IStringDiff
{
    public StringDiffOptions? Options { get; } = options;

    public TextDiff ComputeDiff(string sourceText, string targetText)
    {
        // If no tokenizer specified, use CharDiff for character-level diffing
        if (Options?.TokenBoundaryDetector == null)
        {
            return new CharDiff().ComputeDiff(sourceText, targetText);
        }

        // Use WordDiff with the provided tokenizer
        var wordDiff = new WordDiff(Options.TokenBoundaryDetector);
        var wordEdits = wordDiff.ComputeDiff(sourceText, targetText);
        var result = wordEdits.Select(e => e.ToStringEdit()).ToList();
        return new TextDiff(sourceText, targetText, result);
    }
}
