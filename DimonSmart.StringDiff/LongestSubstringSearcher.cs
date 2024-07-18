using System.Text.RegularExpressions;

namespace DimonSmart.StringDiff;

public static class LongestSubstringSearcher
{
    public record SubstringDescription(int SourceStartIndex, int TargetStartIndex, int Length);

    public delegate HashSet<int> WordBoundaryDetector(string s);

    public static SubstringDescription GetLongestCommonSubstring(string source, string target, StringDiffOptions options) => options.WordBoundaryDetector == null
            ? GetLongestCommonSubstringWithoutWorkdBreaker(source, target, options)
            : GetLongestCommonSubstringWithWordBreaker(source, target, options);

    public static SubstringDescription GetLongestCommonSubstringWithoutWorkdBreaker(string source, string target, StringDiffOptions options)
    {
        var lcsMatrix = new int[source.Length + 1, target.Length + 1];
        var longestLength = 0;
        var sourceEndIndex = 0;
        var targetEndIndex = 0;

        var sourceBoundariesBeginnings = options.WordBoundaryDetector?.DetectWordBeginnings(source);
        var sourceBoundariesEndings = options.WordBoundaryDetector?.DetectWordEndings(source);
        var targetBoundariesBeginnings = options.WordBoundaryDetector?.DetectWordBeginnings(target);
        var targetBoundariesEndings = options.WordBoundaryDetector?.DetectWordEndings(target);

        for (var i = 1; i <= source.Length; i++)
        {
            for (var j = 1; j <= target.Length; j++)
            {
                if (source[i - 1] == target[j - 1])
                {
                    lcsMatrix[i, j] = lcsMatrix[i - 1, j - 1] + 1;

                    var sourceStartIndex = i - lcsMatrix[i, j];
                    var targetStartIndex = j - lcsMatrix[i, j];
                    if (lcsMatrix[i, j] > longestLength)
                    {
                        var longest = lcsMatrix[i, j];
                        if (options.MinCommonLength == 0 || longest > options.MinCommonLength)
                        {
                            if (longest == 5)
                            {

                                var x = 5;
                            }

                            var sourceBeginning = i - longest; if (sourceBeginning < 0) sourceBeginning = 0;
                            var targetBeginning = j - longest; if (targetBeginning < 0) targetBeginning = 0;

                            var isSourceBoundaryBeginningValid = false;
                            var isSourceBoundaryEndingValid = false;
                            var isTargetBoundaryBeginningValid = false;
                            var isTargetBoundaryEndingValid = false;

                            if (options.WordBoundaryDetector != null)
                            {
                                isSourceBoundaryBeginningValid = sourceBoundariesBeginnings!.Contains(sourceBeginning);
                                isSourceBoundaryEndingValid = sourceBoundariesEndings!.Contains(i - 1);
                                isTargetBoundaryBeginningValid = targetBoundariesBeginnings!.Contains(targetBeginning);
                                isTargetBoundaryEndingValid = targetBoundariesEndings!.Contains(j - 1);
                            }
                            if (options.WordBoundaryDetector == null ||
                                (isSourceBoundaryBeginningValid &&
                                isSourceBoundaryEndingValid &&
                                isTargetBoundaryBeginningValid &&
                                isTargetBoundaryEndingValid))
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



    public static SubstringDescription GetLongestCommonSubstringWithWordBreaker(string source, string target, StringDiffOptions options)
    {
        var longestLength = 0;
        var sourceStartIndexResult = 0;
        var targetStartIndexResult = 0;

        var sourceBoundariesBeginnings = options.WordBoundaryDetector?.DetectWordBeginnings(source) ?? new HashSet<int>();
        var sourceBoundariesEndings = options.WordBoundaryDetector?.DetectWordEndings(source) ?? new HashSet<int>();
        var targetBoundariesBeginnings = options.WordBoundaryDetector?.DetectWordBeginnings(target) ?? new HashSet<int>();
        var targetBoundariesEndings = options.WordBoundaryDetector?.DetectWordEndings(target) ?? new HashSet<int>();

        foreach (var sourceStart in sourceBoundariesBeginnings)
        {
            foreach (var targetStart in targetBoundariesBeginnings)
            {
                var possibleSourceLengths = sourceBoundariesEndings.Where(e => e >= sourceStart).Select(e => e - sourceStart + 1).ToList();
                var possibleTargetLengths = targetBoundariesEndings.Where(e => e >= targetStart).Select(e => e - targetStart + 1).ToList();

                var commonLengths = possibleSourceLengths.Intersect(possibleTargetLengths).ToList();

                foreach (int length in commonLengths)
                {
                    int sourceEnd = sourceStart + length - 1;
                    int targetEnd = targetStart + length - 1;

                    if (sourceEnd <= sourceBoundariesEndings.Max() && targetEnd <= targetBoundariesEndings.Max())
                    {
                        string sourceSubstring = source.Substring(sourceStart, length);
                        string targetSubstring = target.Substring(targetStart, length);

                        if (sourceSubstring == targetSubstring && length > longestLength)
                        {
                            longestLength = length;
                            sourceStartIndexResult = sourceStart;
                            targetStartIndexResult = targetStart;
                        }
                    }
                }
            }
        }

        return new SubstringDescription(sourceStartIndexResult, targetStartIndexResult, longestLength);
    }



}