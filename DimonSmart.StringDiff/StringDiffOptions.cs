namespace DimonSmart.StringDiff;

public record StringDiffOptions(
    ITokenizer? Tokenizer = null,
    IEnumerable<IEntityDetector>? EntityDetectors = null)
{
    public ITokenizer GetTokenizer()
    {
        var baseTokenizer = Tokenizer ?? SimpleTokenizer.Instance;
        
        if (EntityDetectors == null || !EntityDetectors.Any())
        {
            return baseTokenizer;
        }
        
        return new TokenProcessor(baseTokenizer, EntityDetectors);
    }
}
