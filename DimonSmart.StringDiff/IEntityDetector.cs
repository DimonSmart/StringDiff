namespace DimonSmart.StringDiff;

public interface IEntityDetector
{
    /// <summary>
    /// Detects specific entities in text and returns their ranges.
    /// These ranges represent regions that should be treated as atomic units and not split further.
    /// </summary>
    void DetectEntities(ReadOnlySpan<char> text, Span<Range> entityRanges, out int entityCount);
}