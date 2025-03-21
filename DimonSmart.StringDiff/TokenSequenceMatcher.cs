using System.Runtime.CompilerServices;

namespace DimonSmart.StringDiff;

public static class TokenSequenceMatcher
{
    public record struct SubstringDescription(int SourceStartIndex, int TargetStartIndex, int Length);

    public static SubstringDescription GetLongestCommonSubstring(ReadOnlySpan<char> source, ReadOnlySpan<char> target, StringDiffOptions? options)
    {
        if (options?.TokenBoundaryDetector == null)
        {
            return FindCommonSpan(source, target);
        }

        // Use stackalloc for small inputs to avoid heap allocations
        const int stackAllocThreshold = 256;
        var useStack = source.Length <= stackAllocThreshold;
        
        Span<Range> sourceRanges = useStack ? stackalloc Range[source.Length] : new Range[source.Length];
        Span<Range> targetRanges = useStack ? stackalloc Range[target.Length] : new Range[target.Length];

        options.TokenBoundaryDetector.TokenizeSpan(source, sourceRanges, out var sourceTokenCount);
        options.TokenBoundaryDetector.TokenizeSpan(target, targetRanges, out var targetTokenCount);

        var result = FindCommonRanges(source, target, sourceRanges[..sourceTokenCount], targetRanges[..targetTokenCount]);
        
        if (!useStack)
        {
            // Help GC by clearing references we no longer need
            sourceRanges = default;
            targetRanges = default;
        }

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static SubstringDescription FindCommonSpan(ReadOnlySpan<char> source, ReadOnlySpan<char> target)
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
                if (source[i - 1] == target[j - 1])
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

        return new SubstringDescription(
            sourceEndIndex - maxLength,
            targetEndIndex - maxLength,
            maxLength);
    }

    private static SubstringDescription FindCommonRanges(
        ReadOnlySpan<char> sourceText,
        ReadOnlySpan<char> targetText,
        ReadOnlySpan<Range> sourceRanges,
        ReadOnlySpan<Range> targetRanges)
    {
        const int stackAllocThreshold = 1024;
        var totalSize = (sourceRanges.Length + 1) * (targetRanges.Length + 1);
        var useStack = totalSize <= stackAllocThreshold;
        
        Span<int> matrixSpan = useStack ? stackalloc int[totalSize] : new int[totalSize];
        
        var maxLength = 0;
        var sourceEndIndex = 0;
        var targetEndIndex = 0;

        for (var i = 1; i <= sourceRanges.Length; i++)
        {
            var sourceRange = sourceRanges[i - 1];
            var sourceSpan = sourceText[sourceRange];
            
            for (var j = 1; j <= targetRanges.Length; j++)
            {
                var targetRange = targetRanges[j - 1];
                var targetSpan = targetText[targetRange];
                
                if (sourceSpan.SequenceEqual(targetSpan))
                {
                    var idx = i * (targetRanges.Length + 1) + j;
                    var prevIdx = (i - 1) * (targetRanges.Length + 1) + (j - 1);
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

        // Convert token indices to character positions
        var sourceStart = 0;
        for (var i = 0; i < sourceEndIndex - maxLength; i++)
        {
            sourceStart += sourceText[sourceRanges[i]].Length;
        }

        var targetStart = 0;
        for (var i = 0; i < targetEndIndex - maxLength; i++)
        {
            targetStart += targetText[targetRanges[i]].Length;
        }

        // Calculate total length of matched tokens
        var length = 0;
        for (var i = sourceEndIndex - maxLength; i < sourceEndIndex; i++)
        {
            length += sourceText[sourceRanges[i]].Length;
        }

        return new SubstringDescription(sourceStart, targetStart, length);
    }
}