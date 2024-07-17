using System.Text.RegularExpressions;

namespace DimonSmart.StringDiff;

public static class LongestSubstringSearcher
{
    public record SubstringDescription(int SourceStartIndex, int TargetStartIndex, int Length);

    public delegate HashSet<int> WordBoundaryDetector(string s);

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
                    if (lcsMatrix[i, j] >= longestLength)
                    {
                        var longest = lcsMatrix[i, j];
                        if (options.MinCommonLength == 0 || longest > options.MinCommonLength)
                        {
                            if (longest == 5)
                            {

                                var x = 5;
                            }


                            var start = i - longest; if (start < 0) start = 0;

                            var isSourceBoundaryValid = sourceBoundaries == null || (sourceBoundaries.Contains(i-1) && sourceBoundaries.Contains(start));
                            var isTargetBoundaryValid = targetBoundaries == null || (targetBoundaries.Contains(j-1) && targetBoundaries.Contains(start));
                            var islongerThenOneChar = options.WordBoundaryDetector == null || longest > 1;
                            if (isSourceBoundaryValid && isTargetBoundaryValid && islongerThenOneChar)
                            {
                                longestLength = longest;
                                sourceEndIndex = i;
                                targetEndIndex = j;
                            }
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