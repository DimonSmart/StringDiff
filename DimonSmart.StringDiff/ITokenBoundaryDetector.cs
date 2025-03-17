namespace DimonSmart.StringDiff
{
    // Interface for tokenization strategy
    public interface ITokenBoundaryDetector
    {
        IEnumerable<string> Tokenize(string text);
    }
}
