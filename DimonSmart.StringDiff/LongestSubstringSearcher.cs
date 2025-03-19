namespace DimonSmart.StringDiff;

public static class LongestSubstringSearcher
{
    public record SubstringDescription(int SourceStartIndex, int TargetStartIndex, int Length);

    public static SubstringDescription GetLongestCommonSubstring(string source, string target, StringDiffOptions options)
    {
        if (options.TokenBoundaryDetector == null)
        {
            return FindCommon(
                source.ToCharArray(),
                target.ToCharArray(),
                (a, b) => a == b,
                options.MinCommonLength);
        }

        return FindCommon(
            options.TokenBoundaryDetector.Tokenize(source).ToArray(),
            options.TokenBoundaryDetector.Tokenize(target).ToArray(),
            string.Equals,
            options.MinCommonLength);
    }

    private static SubstringDescription FindCommon<T>(
        T[] source,
        T[] target,
        Func<T, T, bool> comparer,
        int minCommonLength)
    {
        var matrix = new int[source.Length + 1, target.Length + 1];
        var maxLength = 0;
        var sourceEndIndex = 0;
        var targetEndIndex = 0;

        for (var i = 1; i <= source.Length; i++)
        {
            for (var j = 1; j <= target.Length; j++)
            {
                if (comparer(source[i - 1], target[j - 1]))
                {
                    matrix[i, j] = matrix[i - 1, j - 1] + 1;

                    if (matrix[i, j] > maxLength && (minCommonLength == 0 || matrix[i, j] > minCommonLength))
                    {
                        maxLength = matrix[i, j];
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
}
