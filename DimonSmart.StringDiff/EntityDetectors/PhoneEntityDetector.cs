using System.Text.RegularExpressions;

namespace DimonSmart.StringDiff.EntityDetectors;

public class PhoneEntityDetector : IEntityDetector
{
    private static readonly Regex PhoneRegex = new Regex(@"\+?\d[\d\s\-]{7,}\d", RegexOptions.Compiled);

    public void DetectEntities(ReadOnlySpan<char> text, Span<Range> entityRanges, out int entityCount)
    {
        entityCount = 0;
        var input = text.ToString();
        var matches = PhoneRegex.Matches(input);
        foreach (Match match in matches)
        {
            entityRanges[entityCount++] = new Range(match.Index, match.Index + match.Length);
        }
    }
}