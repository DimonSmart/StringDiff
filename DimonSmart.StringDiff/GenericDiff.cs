namespace DimonSmart.StringDiff;

public class GenericDiff<T> where T : notnull
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
        using var matrixLease = MatrixPool.Rent(source.Length, target.Length);
        var matrixSpan = matrixLease.Span;

        var maxLength = 0;
        var sourceEndIndex = 0;
        var targetEndIndex = 0;

        for (var i = 1; i <= source.Length; i++)
        {
            for (var j = 1; j <= target.Length; j++)
            {
                if (Comparer.Equals(source[i - 1], target[j - 1]))
                {
                    var idx = i * (target.Length + 1) + j;
                    var prevIdx = (i - 1) * (target.Length + 1) + (j - 1);
                    matrixSpan[idx] = matrixSpan[prevIdx] + 1;

                    if (matrixSpan[idx] > maxLength)
                    {
                        maxLength = matrixSpan[idx];
                        sourceEndIndex = i;
                        targetEndIndex = j;
                    }
                }
            }
        }

        return new TokenSequenceMatcher.SubstringDescription(
            sourceEndIndex - maxLength,
            targetEndIndex - maxLength,
            maxLength);
    }
}
