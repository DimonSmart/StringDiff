using System.Text.RegularExpressions;

namespace DimonSmart.StringDiff;

public static class LongestSubstringSearcher
{
    public record SubstringDescription(int SourceStartIndex, int TargetStartIndex, int Length);

    public delegate HashSet<int> WordBoundaryDetector(string s);

    private static readonly Regex WordBoundaryRegex = new(@"\b", RegexOptions.Compiled);

    public static SubstringDescription GetLongestCommonSubstring(string source, string target, StringDiffOptions options)
    {
        var lcsMatrix = new int[source.Length + 1, target.Length + 1];
        var longestLength = 0;
        var sourceEndIndex = 0;
        var targetEndIndex = 0;

        var sourceBoundaries = options.WordBoundaryDetector?.Detect(source);
        var targetBoundaries = options.WordBoundaryDetector?.Detect(target);

        for (var i = 1; i <= source.Length; i++)
        {
            for (var j = 1; j <= target.Length; j++)
            {
                if (source[i - 1] == target[j - 1])
                {
                    lcsMatrix[i, j] = lcsMatrix[i - 1, j - 1] + 1;

                    var sourceStartIndex = i - lcsMatrix[i, j];
                    var targetStartIndex = j - lcsMatrix[i, j];

                    var isSourceBoundaryValid = sourceBoundaries == null || (sourceBoundaries.Contains(sourceStartIndex) && sourceBoundaries.Contains(i));
                    var isTargetBoundaryValid = targetBoundaries == null || (targetBoundaries.Contains(targetStartIndex) && targetBoundaries.Contains(j));

                    if (lcsMatrix[i, j] > longestLength && isSourceBoundaryValid && isTargetBoundaryValid)
                    {
                        var longest = lcsMatrix[i, j];
                        if (options.MinCommonLength == 0 ||
                            (
                             (sourceEndIndex - longest) > options.MinCommonLength &&
                             (source.Length - longest) > options.MinCommonLength
                            ))
                        {
                            longestLength = longest;
                            sourceEndIndex = i;
                            targetEndIndex = j;
                        }
                    }
                }
            }
        }

        var sourceStartIndexResult = sourceEndIndex - longestLength;
        var targetStartIndexResult = targetEndIndex - longestLength;

        return new SubstringDescription(sourceStartIndexResult, targetStartIndexResult, longestLength);
    }
}