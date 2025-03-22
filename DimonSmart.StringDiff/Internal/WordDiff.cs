namespace DimonSmart.StringDiff.Internal;

internal class WordDiff(ITokenizer tokenizer)
{
    private const int StackAllocThreshold = 256;
    private static readonly ReadOnlyMemory<string> EmptyMemoryString = ReadOnlyMemory<string>.Empty;
    public ITokenizer Tokenizer { get; } = tokenizer;
    private ReadOnlyMemory<string> _sourceTokensMemory = EmptyMemoryString;
    private readonly ReadOnlyMemory<string> _targetTokensMemory;
    private readonly int[] _sourceToTargetMap = Array.Empty<int>();
    private readonly int[] _targetToSourceMap = Array.Empty<int>();

    public IReadOnlyCollection<GenericTextEdit<string>> ComputeDiff(string sourceText, string targetText)
    {
        // Use stackalloc for small inputs to avoid heap allocations
        var useStack = sourceText.Length <= StackAllocThreshold && targetText.Length <= StackAllocThreshold;
        Span<Range> sourceRanges = useStack ? stackalloc Range[sourceText.Length] : new Range[sourceText.Length];
        Span<Range> targetRanges = useStack ? stackalloc Range[targetText.Length] : new Range[targetText.Length];

        Tokenizer.TokenizeSpan(sourceText.AsSpan(), sourceRanges, out var sourceTokenCount);
        Tokenizer.TokenizeSpan(targetText.AsSpan(), targetRanges, out var targetTokenCount);

        var sourceTokens = ExtractTokens(sourceText, sourceRanges[..sourceTokenCount]);
        var targetTokens = ExtractTokens(targetText, targetRanges[..targetTokenCount]);

        _sourceTokensMemory = sourceTokens;

        var edits = new List<GenericTextEditSpan<string>>();
        DiffSpans(sourceTokens, targetTokens, 0, edits);

        return edits.Select(e => e.ToGenericTextEdit()).ToList();
    }

    private static ReadOnlyMemory<string> ExtractTokens(string text, ReadOnlySpan<Range> ranges)
    {
        var tokens = new string[ranges.Length];
        for (var i = 0; i < ranges.Length; i++)
        {
            tokens[i] = text[ranges[i]];
        }
        return tokens;
    }

    private void DiffSpans(
        ReadOnlyMemory<string> source,
        ReadOnlyMemory<string> target,
        int offset,
        List<GenericTextEditSpan<string>> edits)
    {
        if (source.Length == 0 && target.Length == 0)
        {
            return;
        }

        if (source.Length == 0)
        {
            edits.Add(new GenericTextEditSpan<string>(offset, EmptyMemoryString, target, _sourceTokensMemory));
            return;
        }

        if (target.Length == 0)
        {
            edits.Add(new GenericTextEditSpan<string>(offset, source, EmptyMemoryString, _sourceTokensMemory));
            return;
        }

        var common = GetLongestCommonSubstring(source.Span, target.Span);

        if (common.Length == 0)
        {
            edits.Add(new GenericTextEditSpan<string>(offset, source, target, _sourceTokensMemory));
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

    private static TokenSequenceMatcher.SubstringDescription GetLongestCommonSubstring(
        ReadOnlySpan<string> source,
        ReadOnlySpan<string> target)
    {
        var maxLength = 0;
        var sourceStart = 0;
        var targetStart = 0;

        // For each possible starting position in source
        for (var i = 0; i < source.Length; i++)
        {
            // For each possible starting position in target
            for (var j = 0; j < target.Length; j++)
            {
                // Try to find the longest common sequence starting at these positions
                var length = 0;
                while (i + length < source.Length &&
                       j + length < target.Length &&
                       source[i + length] == target[j + length])
                {
                    length++;
                }

                // Update if we found a longer sequence
                if (length > maxLength)
                {
                    maxLength = length;
                    sourceStart = i;
                    targetStart = j;
                }
            }
        }

        return new TokenSequenceMatcher.SubstringDescription(sourceStart, targetStart, maxLength);
    }
}
