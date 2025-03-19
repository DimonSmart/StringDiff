namespace DimonSmart.StringDiff;

public static class TokenSequenceMatcher
{
    public record SubstringDescription(int SourceStartIndex, int TargetStartIndex, int Length);

    public static SubstringDescription GetLongestCommonSubstring(ReadOnlySpan<char> source, ReadOnlySpan<char> target, StringDiffOptions? options)
    {
        if (options?.TokenBoundaryDetector == null)
        {
            return FindCommon(source, target, (a, b) => a == b);
        }

        // For token-based comparison, we still need to use strings due to tokenization
        var sourceStr = source.ToString();
        var targetStr = target.ToString();
        var sourceTokens = options.TokenBoundaryDetector.Tokenize(sourceStr).ToArray();
        var targetTokens = options.TokenBoundaryDetector.Tokenize(targetStr).ToArray();

        // Find token-based positions using array overload
        var result = FindCommonArray(sourceTokens, targetTokens, string.Equals);

        // Convert token positions to character positions
        var sourceCharStart = sourceTokens.Take(result.SourceStartIndex).Sum(t => t.Length);
        var targetCharStart = targetTokens.Take(result.TargetStartIndex).Sum(t => t.Length);
        var charLength = sourceTokens.Skip(result.SourceStartIndex).Take(result.Length).Sum(t => t.Length);

        return new SubstringDescription(sourceCharStart, targetCharStart, charLength);
    }

    private static SubstringDescription FindCommon<T>(
        ReadOnlySpan<T> source,
        ReadOnlySpan<T> target,
        Func<T, T, bool> comparer)
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
                if (comparer(source[i - 1], target[j - 1]))
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

        return new SubstringDescription(
            sourceEndIndex - maxLength,
            targetEndIndex - maxLength,
            maxLength);
    }

    // Separate method for array-based comparison to avoid type inference issues
    private static SubstringDescription FindCommonArray<T>(
        T[] source,
        T[] target,
        Func<T, T, bool> comparer)
    {
        return FindCommon(new ReadOnlySpan<T>(source), new ReadOnlySpan<T>(target), comparer);
    }
}