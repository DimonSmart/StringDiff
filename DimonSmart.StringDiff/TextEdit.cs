namespace DimonSmart.StringDiff;

public record TextEdit(int StartPosition, int DeletedLength, string InsertedText);

public record TextEdit<T>(int StartPosition, IReadOnlyList<T> DeletedTokens, IReadOnlyList<T> InsertedTokens, IReadOnlyList<T> SourceTokens)
{
    public TextEdit ToStringEdit()
    {
        var deletedText = string.Join("", DeletedTokens);
        var insertedText = string.Join("", InsertedTokens);

        // Calculate character position by summing lengths of tokens from the original source
        var charPosition = SourceTokens
            .Take(StartPosition)
            .Select(t => t?.ToString() ?? "")
            .Sum(s => s.Length);

        return new TextEdit(charPosition, deletedText.Length, insertedText);
    }

    public static TextEdit<T> FromStringEdit(TextEdit edit, IReadOnlyList<T> sourceTokens, int tokenStartPosition, int tokenCount)
    {
        return new TextEdit<T>(
            tokenStartPosition,
            sourceTokens.Skip(tokenStartPosition).Take(tokenCount).ToList(),
            new[] { edit.InsertedText }.Cast<T>().ToList(),
            sourceTokens
        );
    }
}
