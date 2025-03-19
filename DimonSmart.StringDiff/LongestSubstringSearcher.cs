using System.Runtime.CompilerServices;

namespace DimonSmart.StringDiff;

public static class LongestSubstringSearcher
{
    public record SubstringDescription(int SourceStartIndex, int TargetStartIndex, int Length);

    //public static SubstringDescription GetLongestCommonSubstring(string source, string target, StringDiffOptions options)
    //{
    //    if (options.TokenBoundaryDetector == null)
    //    {
    //        return FindCommon(
    //            source.AsSpan(),
    //            target.AsSpan(),
    //            (a, b) => a == b);
    //    }

    //    var sourceSpan = source.AsSpan();
    //    var targetSpan = target.AsSpan();

    //    Span<Range> sourceRanges = sourceSpan.Length <= 256 ? stackalloc Range[sourceSpan.Length] : new Range[sourceSpan.Length];
    //    Span<Range> targetRanges = targetSpan.Length <= 256 ? stackalloc Range[targetSpan.Length] : new Range[targetSpan.Length];

    //    options.TokenBoundaryDetector.TokenizeSpan(sourceSpan, sourceRanges, out var sourceTokenCount);
    //    options.TokenBoundaryDetector.TokenizeSpan(targetSpan, targetRanges, out var targetTokenCount);

    //    var result = FindCommonRanges(sourceSpan, targetSpan, sourceRanges[..sourceTokenCount], targetRanges[..targetTokenCount]);

    //    if (sourceSpan.Length > 256)
    //    {
    //        sourceRanges = default;
    //        targetRanges = default;
    //    }

    //    return result;
    //}

    //private static SubstringDescription FindCommonRanges(
    //    ReadOnlySpan<char> sourceText,
    //    ReadOnlySpan<char> targetText,
    //    ReadOnlySpan<Range> sourceRanges,
    //    ReadOnlySpan<Range> targetRanges)
    //{
    //    using var matrixLease = MatrixPool.Rent(sourceRanges.Length, targetRanges.Length);
    //    var matrixSpan = matrixLease.Span;

    //    var maxLength = 0;
    //    var sourceEndIndex = 0;
    //    var targetEndIndex = 0;

    //    for (var i = 1; i <= sourceRanges.Length; i++)
    //    {
    //        var sourceRange = sourceRanges[i - 1];
    //        var sourceSpan = sourceText[sourceRange];

    //        for (var j = 1; j <= targetRanges.Length; j++)
    //        {
    //            var targetRange = targetRanges[j - 1];
    //            var targetSpan = targetText[targetRange];

    //            if (sourceSpan.SequenceEqual(targetSpan))
    //            {
    //                var idx = i * (targetRanges.Length + 1) + j;
    //                var prevIdx = (i - 1) * (targetRanges.Length + 1) + (j - 1);
    //                matrixSpan[idx] = matrixSpan[prevIdx] + 1;

    //                if (matrixSpan[idx] > maxLength)
    //                {
    //                    maxLength = matrixSpan[idx];
    //                    sourceEndIndex = i;
    //                    targetEndIndex = j;
    //                }
    //            }
    //        }
    //    }

    //    var sourceStart = 0;
    //    for (var i = 0; i < sourceEndIndex - maxLength; i++)
    //    {
    //        sourceStart += sourceText[sourceRanges[i]].Length;
    //    }

    //    var targetStart = 0;
    //    for (var i = 0; i < targetEndIndex - maxLength; i++)
    //    {
    //        targetStart += targetText[targetRanges[i]].Length;
    //    }

    //    // Calculate total length of matched tokens
    //    var length = 0;
    //    for (var i = sourceEndIndex - maxLength; i < sourceEndIndex; i++)
    //    {
    //        length += sourceText[sourceRanges[i]].Length;
    //    }

    //    return new SubstringDescription(sourceStart, targetStart, length);
    //}

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //private static SubstringDescription FindCommon(
    //    ReadOnlySpan<char> source,
    //    ReadOnlySpan<char> target,
    //    Func<char, char, bool> comparer)
    //{
    //    using var matrixLease = MatrixPool.Rent(source.Length, target.Length);
    //    var matrixSpan = matrixLease.Span;

    //    var maxLength = 0;
    //    var sourceEndIndex = 0;
    //    var targetEndIndex = 0;

    //    for (var i = 1; i <= source.Length; i++)
    //    {
    //        for (var j = 1; j <= target.Length; j++)
    //        {
    //            if (comparer(source[i - 1], target[j - 1]))
    //            {
    //                var idx = i * (target.Length + 1) + j;
    //                var prevIdx = (i - 1) * (target.Length + 1) + (j - 1);
    //                matrixSpan[idx] = matrixSpan[prevIdx] + 1;

    //                if (matrixSpan[idx] > maxLength)
    //                {
    //                    maxLength = matrixSpan[idx];
    //                    sourceEndIndex = i;
    //                    targetEndIndex = j;
    //                }
    //            }
    //        }
    //    }

    //    return new SubstringDescription(
    //        sourceEndIndex - maxLength,
    //        targetEndIndex - maxLength,
    //        maxLength);
    //}
}
