namespace DimonSmart.StringDiff
{
    /// <summary>
    /// Defines a strategy for tokenizing text.
    /// For example, the text "Hello, World!" is tokenized into ["Hello", ",", " ", "World", "!"].
    /// </summary>
    public interface ITokenBoundaryDetector
    {
        /// <summary>
        /// Splits the input text into tokens. Each token is either a sequence of letters and digits 
        /// or a single non-letter/digit character.
        /// </summary>
        /// <param name="text">The text to tokenize.</param>
        /// <returns>An enumerable collection of token strings.</returns>
        IEnumerable<string> Tokenize(string text);
    }
}
