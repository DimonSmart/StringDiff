namespace DimonSmart.StringDiff.Internal;

internal class WordDiff(ITokenizer tokenizer)
{
    private static readonly ReadOnlyMemory<string> EmptyMemoryString = Array.Empty<string>();
    private ReadOnlyMemory<string> _sourceTokensMemory;

    public IReadOnlyCollection<GenericTextEdit<string>> ComputeDiff(string sourceText, string targetText)
    {
        // Pre-allocate token ranges with a reasonable size
        var maxLength = Math.Max(sourceText.Length, targetText.Length);
        Span<Range> sourceRanges = stackalloc Range[maxLength];
        Span<Range> targetRanges = stackalloc Range[maxLength];

        tokenizer.TokenizeSpan(sourceText, sourceRanges, out var sourceCount);
        tokenizer.TokenizeSpan(targetText, targetRanges, out var targetCount);

        // Convert ranges to tokens
        var sourceTokens = new string[sourceCount];
        var targetTokens = new string[targetCount];

        for (var i = 0; i < sourceCount; i++)
        {
            sourceTokens[i] = sourceText[sourceRanges[i]].ToString();
        }

        for (var i = 0; i < targetCount; i++)
        {
            targetTokens[i] = targetText[targetRanges[i]].ToString();
        }

        _sourceTokensMemory = sourceTokens;
        var edits = new List<GenericTextEdit<string>>();
        DiffSpans(sourceTokens.AsMemory(), targetTokens.AsMemory(), 0, edits);

        return edits;
    }

    private void DiffSpans(
        ReadOnlyMemory<string> source,
        ReadOnlyMemory<string> target,
        int offset,
        List<GenericTextEdit<string>> edits)
    {
        if (source.Length == 0 && target.Length == 0)
        {
            return;
        }

        if (source.Length == 0)
        {
            edits.Add(new GenericTextEdit<string>(offset, EmptyMemoryString, target, _sourceTokensMemory));
            return;
        }

        if (target.Length == 0)
        {
            edits.Add(new GenericTextEdit<string>(offset, source, EmptyMemoryString, _sourceTokensMemory));
            return;
        }

        var common = GetLongestCommonSubstring(source.Span, target.Span);

        if (common.Length == 0)
        {
            edits.Add(new GenericTextEdit<string>(offset, source, target, _sourceTokensMemory));
            return;
        }

        // Process the part before the common substring
        DiffSpans(
            source.Slice(0, common.SourceStart),
            target.Slice(0, common.TargetStart),
            offset,
            edits);

        // Process the part after the common substring
        DiffSpans(
            source.Slice(common.SourceStart + common.Length),
            target.Slice(common.TargetStart + common.Length),
            offset + common.SourceStart + common.Length,
            edits);
    }

    private static CommonSubstring GetLongestCommonSubstring(ReadOnlySpan<string> source, ReadOnlySpan<string> target)
    {
        var maxLength = 0;
        var sourceStart = 0;
        var targetStart = 0;

        for (var i = 0; i < source.Length; i++)
        {
            for (var j = 0; j < target.Length; j++)
            {
                var length = 0;
                while (i + length < source.Length &&
                       j + length < target.Length &&
                       source[i + length].Equals(target[j + length], StringComparison.Ordinal))
                {
                    length++;
                }

                if (length > maxLength)
                {
                    maxLength = length;
                    sourceStart = i;
                    targetStart = j;
                }
            }
        }

        return new CommonSubstring(maxLength, sourceStart, targetStart);
    }

    private readonly struct CommonSubstring(int length, int sourceStart, int targetStart)
    {
        public int Length { get; } = length;
        public int SourceStart { get; } = sourceStart;
        public int TargetStart { get; } = targetStart;
    }
}
