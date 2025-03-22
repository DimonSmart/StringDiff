using System.Text.RegularExpressions;

namespace DimonSmart.StringDiff.EntityDetectors;

public class EmailEntityDetector : IEntityDetector
{
    private static readonly Regex EmailRegex = new Regex(@"[A-Za-z0-9._%+-]+@[A-Za-z0-9.-]+\.[A-Za-z]{2,}", RegexOptions.Compiled);

    public void DetectEntities(ReadOnlySpan<char> text, Span<Range> entityRanges, out int entityCount)
    {
        entityCount = 0;
        var matches = EmailRegex.Matches(text.ToString());
        foreach (Match match in matches)
        {
            entityRanges[entityCount++] = new Range(match.Index, match.Index + match.Length);
        }
    }
}