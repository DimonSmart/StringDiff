using DimonSmart.StringDiff.Internal;

namespace DimonSmart.StringDiff;

public class StringDiff : IStringDiff
{
    public StringDiffOptions? Options { get; }

    public StringDiff(StringDiffOptions? options = null) => Options = options;

    public TextDiff ComputeDiff(string sourceText, string targetText)
    {
        if (Options?.Tokenizer == null)
        {
            var charDiff = new CharDiff();
            return charDiff.ComputeDiff(sourceText, targetText);
        }

        var wordDiff = new WordDiff(Options.Tokenizer);
        var genericEdits = wordDiff.ComputeDiff(sourceText, targetText);
        var textEdits = genericEdits.Select(edit => edit.ToStringEdit()).ToList();
        return new TextDiff(sourceText, targetText, textEdits);
    }
}
