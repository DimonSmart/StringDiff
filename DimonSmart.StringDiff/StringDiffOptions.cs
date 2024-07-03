namespace DimonSmart.StringDiff;

/// <param name="MinCommonLength"> String diff internally uses an algorithm to find the longest common substring.
/// This parameter forces the differ to report longer changes.
/// For example, if set to 10 (phone number length), the differ will prefer not to report single-letter changes.
/// This is useful if you are comparing phone numbers. Instead of reporting many single-digit changes,
/// the differ will report the diff as whole line changes. </param>
public record StringDiffOptions(int MinCommonLength);
