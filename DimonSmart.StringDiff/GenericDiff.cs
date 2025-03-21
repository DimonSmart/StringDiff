namespace DimonSmart.StringDiff;

internal class GenericDiff<T> where T : notnull
{
    private const int StackAllocThreshold = 256;
    public ITokenBoundaryDetector Tokenizer { get; }
    public IEqualityComparer<T> Comparer { get; }
    private ReadOnlyMemory<T> SourceTokensMemory { get; set; }

    public GenericDiff(ITokenBoundaryDetector tokenizer, IEqualityComparer<T>? comparer = null)
    {
        Tokenizer = tokenizer;
        Comparer = comparer ?? EqualityComparer<T>.Default;
        SourceTokensMemory = Array.Empty<T>();
    }

    public IReadOnlyCollection<GenericTextEdit<T>> ComputeDiff(string sourceText, string targetText)
    {
        var sourceSpan = sourceText.AsSpan();
        var targetSpan = targetText.AsSpan();
        
        // Use stackalloc for small inputs
        var useStack = sourceSpan.Length <= StackAllocThreshold;

        var sourceRanges = useStack ? stackalloc Range[sourceSpan.Length] : new Range[sourceSpan.Length];
        var targetRanges = useStack ? stackalloc Range[targetSpan.Length] : new Range[targetSpan.Length];

        Tokenizer.TokenizeSpan(sourceSpan, sourceRanges, out var sourceTokenCount);
        Tokenizer.TokenizeSpan(targetSpan, targetRanges, out var targetTokenCount);

        // Convert token ranges to T array
        var sourceTokens = new T[sourceTokenCount];
        var targetTokens = new T[targetTokenCount];

        for (var i = 0; i < sourceTokenCount; i++)
        {
            sourceTokens[i] = (T)(object)sourceSpan[sourceRanges[i]].ToString();
        }
        for (var i = 0; i < targetTokenCount; i++)
        {
            targetTokens[i] = (T)(object)targetSpan[targetRanges[i]].ToString();
        }

        SourceTokensMemory = sourceTokens;
        var spanEdits = new List<GenericTextEditSpan<T>>();
        DiffSpan(sourceTokens.AsMemory(), targetTokens.AsMemory(), 0, spanEdits);

        if (!useStack)
        {
            sourceRanges = default;
            targetRanges = default;
        }

        // Convert span edits to regular edits only at the end
        return spanEdits.Select(e => e.ToGenericTextEdit()).ToList();
    }

    private void DiffSpan(
        ReadOnlyMemory<T> source,
        ReadOnlyMemory<T> target,
        int offset,
        List<GenericTextEditSpan<T>> edits)
    {
        if (source.Length == 0 && target.Length == 0) return;

        if (source.Length == 0)
        {
            edits.Add(new GenericTextEditSpan<T>(
                offset,
                ReadOnlyMemory<T>.Empty,
                target,
                SourceTokensMemory));
            return;
        }

        if (target.Length == 0)
        {
            edits.Add(new GenericTextEditSpan<T>(
                offset,
                source,
                ReadOnlyMemory<T>.Empty,
                SourceTokensMemory));
            return;
        }

        // For completely different strings, check if we should treat them as a single edit
        if (source.Length == 1 && target.Length == 1)
        {
            if (!Comparer.Equals(source.Span[0], target.Span[0]))
            {
                edits.Add(new GenericTextEditSpan<T>(
                    offset,
                    source,
                    target,
                    SourceTokensMemory));
                return;
            }
        }

        var common = GetLongestCommonSubstring(source.Span, target.Span);

        if (common.Length == 0)
        {
            edits.Add(new GenericTextEditSpan<T>(
                offset,
                source,
                target,
                SourceTokensMemory));
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

    private TokenSequenceMatcher.SubstringDescription GetLongestCommonSubstring(
        ReadOnlySpan<T> source,
        ReadOnlySpan<T> target)
    {
        var maxLength = 0;
        var sourceStart = 0;
        var targetStart = 0;

        // Convert tokens to strings for proper comparison
        var sourceTokens = source.ToArray();
        var targetTokens = target.ToArray();

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
                       Comparer.Equals(sourceTokens[i + length], targetTokens[j + length]))
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

    private class TokenCompareDetector : ITokenBoundaryDetector
    {
        private readonly T[] _source;
        private readonly T[] _target;
        private readonly IEqualityComparer<T> _comparer;

        public TokenCompareDetector(T[] source, T[] target, IEqualityComparer<T> comparer)
        {
            _source = source;
            _target = target;
            _comparer = comparer;
        }

        public void TokenizeSpan(ReadOnlySpan<char> text, Span<Range> tokenRanges, out int tokenCount)
        {
            // Each position is a token by itself
            tokenCount = text.Length;
            for (var i = 0; i < tokenCount && i < tokenRanges.Length; i++)
            {
                tokenRanges[i] = new Range(i, i + 1);
            }
        }

        public bool TokensEqual(ReadOnlySpan<char> source, ReadOnlySpan<char> target)
        {
            if (source.Length != 1 || target.Length != 1) return false;
            var sourceIndex = source[0];
            var targetIndex = target[0];
            return sourceIndex < _source.Length && targetIndex < _target.Length && 
                   _comparer.Equals(_source[sourceIndex], _target[targetIndex]);
        }
    }
}
