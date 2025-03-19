namespace DimonSmart.StringDiff;

public class GenericDiff<T>
{
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
        // Use stackalloc for small inputs
        const int stackAllocThreshold = 256;
        bool useStack = sourceText.Length <= stackAllocThreshold;

        Span<Range> sourceRanges = useStack ? stackalloc Range[sourceText.Length] : new Range[sourceText.Length];
        Span<Range> targetRanges = useStack ? stackalloc Range[targetText.Length] : new Range[targetText.Length];

        Tokenizer.TokenizeSpan(sourceText.AsSpan(), sourceRanges, out int sourceTokenCount);
        Tokenizer.TokenizeSpan(targetText.AsSpan(), targetRanges, out int targetTokenCount);

        // Convert ranges to tokens only once
        var sourceTokens = new T[sourceTokenCount];
        var targetTokens = new T[targetTokenCount];
        
        var sourceSpan = sourceText.AsSpan();
        var targetSpan = targetText.AsSpan();
        
        for (int i = 0; i < sourceTokenCount; i++)
        {
            sourceTokens[i] = (T)(object)sourceSpan[sourceRanges[i]].ToString();
        }
        for (int i = 0; i < targetTokenCount; i++)
        {
            targetTokens[i] = (T)(object)targetSpan[targetRanges[i]].ToString();
        }

        SourceTokensMemory = sourceTokens;
        var result = new List<GenericTextEdit<T>>();
        DiffSpan(sourceTokens, targetTokens, 0, result);
        return result;
    }

    private void DiffSpan(
        ReadOnlyMemory<T> source,
        ReadOnlyMemory<T> target,
        int offset,
        List<GenericTextEdit<T>> edits)
    {
        var sourceSpan = source.Span;
        var targetSpan = target.Span;

        if (sourceSpan.IsEmpty && targetSpan.IsEmpty) return;

        if (sourceSpan.IsEmpty)
        {
            edits.Add(new GenericTextEdit<T>(
                offset,
                Array.Empty<T>(),
                target.ToArray(),
                SourceTokensMemory.ToArray()));
            return;
        }

        if (targetSpan.IsEmpty)
        {
            edits.Add(new GenericTextEdit<T>(
                offset,
                source.ToArray(),
                Array.Empty<T>(),
                SourceTokensMemory.ToArray()));
            return;
        }

        var common = GetLongestCommonSubstring(sourceSpan, targetSpan);
        if (common.Length == 0)
        {
            edits.Add(new GenericTextEdit<T>(
                offset,
                source.ToArray(),
                target.ToArray(),
                SourceTokensMemory.ToArray()));
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
        const int stackAllocThreshold = 1024;
        var totalSize = (source.Length + 1) * (target.Length + 1);
        var useStack = totalSize <= stackAllocThreshold;

        Span<int> matrixSpan = useStack ? stackalloc int[totalSize] : new int[totalSize];

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

        if (!useStack)
        {
            matrixSpan = default;
        }

        return new TokenSequenceMatcher.SubstringDescription(
            sourceEndIndex - maxLength,
            targetEndIndex - maxLength,
            maxLength);
    }
}
