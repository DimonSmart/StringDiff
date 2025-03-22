namespace DimonSmart.StringDiff;

public record StringDiffOptions(ITokenizer? Tokenizer = null, IEnumerable<IEntityDetector>? EntityDetectors = null);
