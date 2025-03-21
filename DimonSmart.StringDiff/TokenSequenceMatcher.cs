using System.Runtime.CompilerServices;

namespace DimonSmart.StringDiff;

public static class TokenSequenceMatcher
{
    public record struct SubstringDescription(int SourceStartIndex, int TargetStartIndex, int Length);

    private static int[]? _sharedMatrix;

    public static SubstringDescription GetLongestCommonSubstring(ReadOnlySpan<char> source, ReadOnlySpan<char> target, StringDiffOptions? options)
    {
        var requiredSize = (source.Length + 1) * (target.Length + 1);
        if (_sharedMatrix == null || _sharedMatrix.Length < requiredSize)
        {
            _sharedMatrix = new int[requiredSize];
        }

        // Clear the existing matrix
        _sharedMatrix.AsSpan(0, requiredSize).Clear();
        
        if (options?.TokenBoundaryDetector == null)
        {
            return FindCommonSpanWithMatrix(source, target, _sharedMatrix);
        }

        // Use stackalloc for small inputs to avoid heap allocations
        const int stackAllocThreshold = 256;
        var useStack = source.Length <= stackAllocThreshold;
        
        var sourceRanges = useStack ? stackalloc Range[source.Length] : new Range[source.Length];
        var targetRanges = useStack ? stackalloc Range[target.Length] : new Range[target.Length];

        options.TokenBoundaryDetector.TokenizeSpan(source, sourceRanges, out var sourceTokenCount);
        options.TokenBoundaryDetector.TokenizeSpan(target, targetRanges, out var targetTokenCount);

        var result = FindCommonRangesWithMatrix(source, target, sourceRanges[..sourceTokenCount], targetRanges[..targetTokenCount], _sharedMatrix);
        
        if (!useStack)
        {
            // Help GC by clearing references we no longer need
            sourceRanges = default;
            targetRanges = default;
        }

        return result;
    }

    private static SubstringDescription FindCommonSpanWithMatrix(ReadOnlySpan<char> source, ReadOnlySpan<char> target, Span<int> matrixSpan)
    {
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

        return new SubstringDescription(
            sourceEndIndex - maxLength,
            targetEndIndex - maxLength,
            maxLength);
    }

    private static SubstringDescription FindCommonRangesWithMatrix(
        ReadOnlySpan<char> sourceText,
        ReadOnlySpan<char> targetText,
        ReadOnlySpan<Range> sourceRanges,
        ReadOnlySpan<Range> targetRanges,
        Span<int> matrixSpan)
    {
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

        // Convert token indices to character positions
        var sourceStart = 0;
        for (var i = 0; i < sourceEndIndex - maxLength; i++)
        {
            var range = sourceRanges[i];
            sourceStart = range.End.Value;
        }

        var targetStart = 0;
        for (var i = 0; i < targetEndIndex - maxLength; i++)
        {
            var range = targetRanges[i];
            targetStart = range.End.Value;
        }

        // Calculate total length of matched tokens
        var length = 0;
        for (var i = sourceEndIndex - maxLength; i < sourceEndIndex; i++)
        {
            var range = sourceRanges[i];
            length += range.End.Value - range.Start.Value;
        }

        return new SubstringDescription(sourceStart, targetStart, length);
    }
}