using System.Text.RegularExpressions;

namespace DimonSmart.StringDiff;

public static class DefaultWordBoundaryDetector
{
    private static readonly Regex WordBoundaryRegex = new(@"\b", RegexOptions.Compiled);

    public static HashSet<int> Detect(string s)
    {
        var boundaries = new HashSet<int> { 0 };
        var matches = WordBoundaryRegex.Matches(s);

        foreach (Match match in matches)
            boundaries.Add(match.Index);

        boundaries.Add(s.Length);

        return boundaries;
    }
}