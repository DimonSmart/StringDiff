namespace DimonSmart.StringDiff;

internal class CharDiff
{
    public ITokenBoundaryDetector Tokenizer { get; }
    private ReadOnlyMemory<char> _sourceMemory;

    public CharDiff(ITokenBoundaryDetector tokenizer)
    {
        Tokenizer = tokenizer;
        _sourceMemory = ReadOnlyMemory<char>.Empty;
    }

    public IReadOnlyCollection<GenericTextEdit<char>> ComputeDiff(string sourceText, string targetText)
    {
        var source = sourceText.AsMemory();
        var target = targetText.AsMemory();
        _sourceMemory = source;

        var edits = new List<GenericTextEditSpan<char>>();
        DiffSpans(source.Span, target.Span, 0, edits);

        return edits.Select(e => e.ToGenericTextEdit()).ToList();
    }

    private void DiffSpans(
        ReadOnlySpan<char> source,
        ReadOnlySpan<char> target,
        int offset,
        List<GenericTextEditSpan<char>> edits)
    {
        if (source.Length == 0 && target.Length == 0) return;

        if (source.Length == 0)
        {
            edits.Add(new GenericTextEditSpan<char>(offset, ReadOnlyMemory<char>.Empty, target.ToArray(), _sourceMemory));
            return;
        }

        if (target.Length == 0)
        {
            edits.Add(new GenericTextEditSpan<char>(offset, source.ToArray(), ReadOnlyMemory<char>.Empty, _sourceMemory));
            return;
        }

        var common = TokenSequenceMatcher.GetLongestCommonSubstring(source, target, null);

        if (common.Length == 0)
        {
            edits.Add(new GenericTextEditSpan<char>(offset, source.ToArray(), target.ToArray(), _sourceMemory));
            return;
        }

        // Process the part before the common substring
        DiffSpans(
            source[..common.SourceStartIndex],
            target[..common.TargetStartIndex],
            offset,
            edits);

        // Process the part after the common substring
        DiffSpans(
            source[(common.SourceStartIndex + common.Length)..],
            target[(common.TargetStartIndex + common.Length)..],
            offset + common.SourceStartIndex + common.Length,
            edits);
    }
}