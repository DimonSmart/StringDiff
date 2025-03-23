using System.Runtime.CompilerServices;

namespace DimonSmart.StringDiff;

public static class LongestMatchFinder
{
    public record struct SubstringDescription(int SourceStartIndex, int TargetStartIndex, int Length);

    private static int[]? _dpMatrixCache;

    public static SubstringDescription GetLongestCommonSubstring(ReadOnlySpan<char> source, ReadOnlySpan<char> target)
    {
        var dpMatrixSize = (source.Length + 1) * (target.Length + 1);
        if (_dpMatrixCache == null || _dpMatrixCache.Length < dpMatrixSize)
        {
            _dpMatrixCache = new int[dpMatrixSize];
        }

        _dpMatrixCache.AsSpan(0, dpMatrixSize).Clear();
        return FindCommonSpanWithMatrix(source, target, _dpMatrixCache);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static SubstringDescription FindCommonSpanWithMatrix(ReadOnlySpan<char> source, ReadOnlySpan<char> target, Span<int> matrixSpan)
    {
        var longestCommonLength = 0;
        var sourceStart = 0;
        var targetStart = 0;

        var rows = source.Length + 1;
        var cols = target.Length + 1;
        
        for (var i = 1; i <= source.Length; i++)
        {
            for (var j = 1; j <= target.Length; j++)
            {
                if (source[i - 1] == target[j - 1])
                {
                    var idx = i * cols + j;
                    var prevIdx = (i - 1) * cols + (j - 1);
                    matrixSpan[idx] = matrixSpan[prevIdx] + 1;

                    if (matrixSpan[idx] > longestCommonLength)
                    {
                        longestCommonLength = matrixSpan[idx];
                        sourceStart = i - longestCommonLength;
                        targetStart = j - longestCommonLength;
                    }
                }
            }
        }

        return new SubstringDescription(sourceStart, targetStart, longestCommonLength);
    }
}