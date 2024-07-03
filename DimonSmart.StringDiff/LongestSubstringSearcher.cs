using System.Text.RegularExpressions;

namespace DimonSmart.StringDiff;

public static class LongestSubstringSearcher
{
    public record SubstringDescription(int SourceStartIndex, int TargetStartIndex, int Length);

    public delegate HashSet<int> WordBoundaryDetector(string s);

    private static readonly Regex WordBoundaryRegex = new(@"\b", RegexOptions.Compiled);

    public static SubstringDescription GetLongestCommonSubstring(string source, string target, WordBoundaryDetector? wordBoundaries = null)
    {
        var sourceBoundaries = wordBoundaries?.Invoke(source);
        var targetBoundaries = wordBoundaries?.Invoke(target);

        var dp = new int[source.Length + 1, target.Length + 1];
        var maxLength = 0;
        var sourceEndAt = 0;
        var targetStartIndex = 0;

        for (var i = 1; i <= source.Length; i++)
        {
            for (var j = 1; j <= target.Length; j++)
            {
                var sourceIsBoundary = sourceBoundaries?.Contains(i - 1) ?? true;
                var targetIsBoundary = targetBoundaries?.Contains(j - 1) ?? true;

                if (source[i - 1] == target[j - 1] && sourceIsBoundary && targetIsBoundary)
                {
                    dp[i, j] = dp[i - 1, j - 1] + 1;
                    if (dp[i, j] <= maxLength) continue;
                    maxLength = dp[i, j];
                    sourceEndAt = i;
                    targetStartIndex = j - maxLength;
                }
            }
        }

        return new SubstringDescription(sourceEndAt - maxLength, targetStartIndex, maxLength);
    }


}