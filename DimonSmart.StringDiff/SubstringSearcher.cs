public static class SubstringSearcher
{
    public record SubstringResult(int SourceStartIndex, int TargetStartIndex, int Length);

    public static SubstringResult LongestCommonSubstring(string source, string target)
    {
        var dp = new int[source.Length + 1, target.Length + 1];
        var maxLength = 0;
        var sourceEndAt = 0;
        var targetStartIndex = 0;

        for (var i = 1; i <= source.Length; i++)
        {
            for (int j = 1; j <= target.Length; j++)
            {
                if (source[i - 1] == target[j - 1])
                {
                    dp[i, j] = dp[i - 1, j - 1] + 1;
                    if (dp[i, j] <= maxLength) continue;
                    maxLength = dp[i, j];
                    sourceEndAt = i;
                    targetStartIndex = j - maxLength;
                }
            }
        }

        return new SubstringResult(sourceEndAt - maxLength, targetStartIndex, maxLength);
    }
}