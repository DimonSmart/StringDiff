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
            sourceRanges = default;
            targetRanges = default;
        }

        return result;
    }

    private static SubstringDescription FindCommonSpanWithMatrix(ReadOnlySpan<char> source, ReadOnlySpan<char> target, Span<int> matrixSpan)
    {
        var maxLength = 0;
        var sourceStart = 0;
        var targetStart = 0;

        // Convert spans to row-major matrix form
        var rows = source.Length + 1;
        var cols = target.Length + 1;
        
        // Fill the matrix
        for (var i = 1; i <= source.Length; i++)
        {
            for (var j = 1; j <= target.Length; j++)
            {
                if (source[i - 1] == target[j - 1])
                {
                    var idx = i * cols + j;
                    var prevIdx = (i - 1) * cols + (j - 1);
                    matrixSpan[idx] = matrixSpan[prevIdx] + 1;

                    if (matrixSpan[idx] > maxLength)
                    {
                        maxLength = matrixSpan[idx];
                        sourceStart = i - maxLength;
                        targetStart = j - maxLength;
                    }
                }
            }
        }

        return new SubstringDescription(sourceStart, targetStart, maxLength);
    }

    private static SubstringDescription FindCommonRangesWithMatrix(
        ReadOnlySpan<char> sourceText,
        ReadOnlySpan<char> targetText,
        ReadOnlySpan<Range> sourceRanges,
        ReadOnlySpan<Range> targetRanges,
        Span<int> matrixSpan)
    {
        var maxLength = 0;
        var sourceStart = 0;
        var targetStart = 0;

        // Convert ranges to row-major matrix form
        var rows = sourceRanges.Length + 1;
        var cols = targetRanges.Length + 1;

        // Fill the matrix
        for (var i = 1; i <= sourceRanges.Length; i++)
        {
            var sourceRange = sourceRanges[i - 1];
            var sourceSlice = sourceText[sourceRange];

            for (var j = 1; j <= targetRanges.Length; j++)
            {
                var targetRange = targetRanges[j - 1];
                var targetSlice = targetText[targetRange];

                if (sourceSlice.SequenceEqual(targetSlice))
                {
                    var idx = i * cols + j;
                    var prevIdx = (i - 1) * cols + (j - 1);
                    matrixSpan[idx] = matrixSpan[prevIdx] + 1;

                    if (matrixSpan[idx] > maxLength)
                    {
                        maxLength = matrixSpan[idx];
                        sourceStart = i - maxLength;
                        targetStart = j - maxLength;
                    }
                }
            }
        }

        // Convert token indices back to character indices
        if (maxLength > 0)
        {
            var actualSourceStart = sourceRanges[sourceStart].Start.Value;
            var actualTargetStart = targetRanges[targetStart].Start.Value;
            var lastSourceToken = sourceRanges[sourceStart + maxLength - 1];
            var actualLength = lastSourceToken.End.Value - actualSourceStart;
            return new SubstringDescription(actualSourceStart, actualTargetStart, actualLength);
        }

        return new SubstringDescription(0, 0, 0);
    }
}