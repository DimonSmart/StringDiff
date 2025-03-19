namespace DimonSmart.StringDiff;

public class StringDiff(StringDiffOptions? options = null) : IStringDiff
{
    public StringDiffOptions? Options { get; } = options;

    public TextDiff ComputeDiff(string sourceText, string targetText)
    {
        if (Options?.TokenBoundaryDetector != null)
        {
            var genericDiff = new GenericDiff<string>(Options.TokenBoundaryDetector, null);
            var genericEdits = genericDiff.ComputeDiff(sourceText, targetText);
            var result = genericEdits.Select(e => e.ToStringEdit()).ToList();
            return new TextDiff(sourceText, targetText, result);
        }

        var edits = new List<TextEdit>();
        DiffSpan(sourceText.AsSpan(), targetText.AsSpan(), 0, edits);
        return new TextDiff(sourceText, targetText, edits);
    }

    private void DiffSpan(ReadOnlySpan<char> source, ReadOnlySpan<char> target, int offset, List<TextEdit> edits)
    {
        if (source.SequenceEqual(target)) return;

        if (source.IsEmpty || target.IsEmpty)
        {
            edits.Add(new TextEdit(offset, source.Length, target.ToString()));
            return;
        }

        var common = TokenSequenceMatcher.GetLongestCommonSubstring(source, target, Options);

        if (common.Length == 0)
        {
            edits.Add(new TextEdit(offset, source.Length, target.ToString()));
            return;
        }

        // Process the part before the common substring
        DiffSpan(
            source[..common.SourceStartIndex],
            target[..common.TargetStartIndex],
            offset,
            edits);

        // Process the part after the common substring
        DiffSpan(
            source[(common.SourceStartIndex + common.Length)..],
            target[(common.TargetStartIndex + common.Length)..],
            offset + common.SourceStartIndex + common.Length,
            edits);
    }
}
