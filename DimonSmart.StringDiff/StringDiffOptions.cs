namespace DimonSmart.StringDiff;

/// <param name="TokenBoundaryDetector"> The token boundary detector to use for detecting word or other token boundaries.
/// If null, the default character-by-character tokenization will be used. </param>
public record StringDiffOptions(ITokenBoundaryDetector? TokenBoundaryDetector = null);
