namespace DimonSmart.StringDiff
{
    // Interface for tokenization strategy
    public interface ITokenBoundaryDetector<T>
    {
        IEnumerable<T> Tokenize(string text);
    }
}
