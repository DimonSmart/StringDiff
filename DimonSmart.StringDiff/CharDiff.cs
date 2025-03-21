namespace DimonSmart.StringDiff;

internal class CharDiff
{
    public ITokenBoundaryDetector Tokenizer { get; }
    private ReadOnlyMemory<char> SourceMemory { get; set; }

    public CharDiff(ITokenBoundaryDetector tokenizer)
    {
        Tokenizer = tokenizer;
        SourceMemory = ReadOnlyMemory<char>.Empty;
    }

    public IReadOnlyCollection<GenericTextEdit<char>> ComputeDiff(string sourceText, string targetText)
    {
        var sourceSpan = sourceText.AsSpan();
        var targetSpan = targetText.AsSpan();
        SourceMemory = sourceText.AsMemory();

        var spanEdits = new List<GenericTextEditSpan<char>>();
        DiffSpan(sourceSpan, targetSpan, 0, spanEdits);

        return spanEdits.Select(edit =>
        {
            var deletedChars = edit.DeletedTokens.ToArray();
            var insertedChars = edit.InsertedTokens.ToArray();
            return new GenericTextEdit<char>(edit.StartPosition, deletedChars, insertedChars, SourceMemory.ToArray());
        }).ToList();
    }

    private void DiffSpan(
        ReadOnlySpan<char> source,
        ReadOnlySpan<char> target,
        int offset,
        List<GenericTextEditSpan<char>> edits)
    {
        if (source.Length == 0 && target.Length == 0) return;

        if (source.Length == 0)
        {
            edits.Add(new GenericTextEditSpan<char>(offset, source.ToArray().AsMemory(), target.ToArray().AsMemory(), SourceMemory));
            return;
        }

        if (target.Length == 0)
        {
            edits.Add(new GenericTextEditSpan<char>(offset, source.ToArray().AsMemory(), target.ToArray().AsMemory(), SourceMemory));
            return;
        }

        var common = TokenSequenceMatcher.GetLongestCommonSubstring(source, target, null);

        if (common.Length == 0)
        {
            edits.Add(new GenericTextEditSpan<char>(offset, source.ToArray().AsMemory(), target.ToArray().AsMemory(), SourceMemory));
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