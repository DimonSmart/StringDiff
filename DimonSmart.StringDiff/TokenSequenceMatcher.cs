namespace DimonSmart.StringDiff;

public static class TokenSequenceMatcher
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

        var sourceTokens = options.TokenBoundaryDetector.Tokenize(source).ToArray();
        var targetTokens = options.TokenBoundaryDetector.Tokenize(target).ToArray();

        // Find token-based positions
        var result = FindCommon(sourceTokens, targetTokens, string.Equals, options.MinCommonLength);

        // Convert token positions to character positions
        var sourceCharStart = sourceTokens.Take(result.SourceStartIndex).Sum(t => t.Length);
        var targetCharStart = targetTokens.Take(result.TargetStartIndex).Sum(t => t.Length);
        var charLength = sourceTokens.Skip(result.SourceStartIndex).Take(result.Length).Sum(t => t.Length);

        return new SubstringDescription(sourceCharStart, targetCharStart, charLength);
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