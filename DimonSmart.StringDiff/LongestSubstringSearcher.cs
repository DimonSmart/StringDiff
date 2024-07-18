namespace DimonSmart.StringDiff;

public static class LongestSubstringSearcher
{
    public record SubstringDescription(int SourceStartIndex, int TargetStartIndex, int Length);

    public delegate HashSet<int> WordBoundaryDetector(string s);

    public static SubstringDescription GetLongestCommonSubstring(string source, string target, StringDiffOptions options) =>
        options.WordBoundaryDetector == null
            ? GetLongestCommonSubstringWithoutWordBoundaryDetector(source, target, options)
            : GetLongestCommonSubstringWithWordBoundaryDetector(source, target, options);

    public static SubstringDescription GetLongestCommonSubstringWithoutWordBoundaryDetector(string source, string target, StringDiffOptions options)
    {
        var lcsMatrix = new int[source.Length + 1, target.Length + 1];
        var longestLength = 0;
        var sourceEndIndex = 0;
        var targetEndIndex = 0;

        for (var i = 1; i <= source.Length; i++)
        {
            for (var j = 1; j <= target.Length; j++)
            {
                if (source[i - 1] == target[j - 1])
                {
                    lcsMatrix[i, j] = lcsMatrix[i - 1, j - 1] + 1;

                    if (lcsMatrix[i, j] > longestLength)
                    {
                        var longest = lcsMatrix[i, j];
                        if (options.MinCommonLength == 0 || longest > options.MinCommonLength)
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


    public static SubstringDescription GetLongestCommonSubstringWithWordBoundaryDetector(string source, string target, StringDiffOptions options)
    {
        var longestLength = 0;
        var sourceStartIndexResult = 0;
        var targetStartIndexResult = 0;
        var sourceBoundariesBeginnings = options.WordBoundaryDetector!.DetectWordBeginnings(source);
        var sourceBoundariesEndings = options.WordBoundaryDetector!.DetectWordEndings(source);
        var targetBoundariesBeginnings = options.WordBoundaryDetector!.DetectWordBeginnings(target);
        var targetBoundariesEndings = options!.WordBoundaryDetector.DetectWordEndings(target);

        foreach (var sourceStart in sourceBoundariesBeginnings)
        {
            foreach (var targetStart in targetBoundariesBeginnings)
            {
                var possibleSourceLengths = sourceBoundariesEndings.Where(e => e >= sourceStart).Select(e => e - sourceStart + 1);
                var possibleTargetLengths = targetBoundariesEndings.Where(e => e >= targetStart).Select(e => e - targetStart + 1);
                var commonLengths = possibleSourceLengths.Intersect(possibleTargetLengths).ToList();

                foreach (var length in commonLengths)
                {
                    var sourceEnd = sourceStart + length - 1;
                    var targetEnd = targetStart + length - 1;

                    if (sourceEnd < source.Length && targetEnd < target.Length)
                    {
                        var sourceSpan = source.AsSpan(sourceStart, length);
                        var targetSpan = target.AsSpan(targetStart, length);

                        if (sourceSpan.SequenceEqual(targetSpan) && length > longestLength)
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
