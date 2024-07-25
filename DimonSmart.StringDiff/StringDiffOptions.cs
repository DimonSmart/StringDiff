namespace DimonSmart.StringDiff;

/// <param name="MinCommonLength"> The string diff internally uses an algorithm to find the longest common substring.
/// This parameter forces the differ to report longer changes.
/// For example, if set to 10 (phone number length), the differ will prefer not to report single-letter changes.
/// This is useful when comparing phone numbers. Instead of reporting many single-digit changes,
/// the differ will report the diff as whole line changes. </param>
/// <param name="WordBoundaryDetector"> The word boundary detector to use for detecting word boundaries.
/// If null, the default word boundary detection will be used. </param>
public record StringDiffOptions(int MinCommonLength, IWordBoundaryDetector? WordBoundaryDetector = null)
{
    public static StringDiffOptions Default => new(0, null);
}
