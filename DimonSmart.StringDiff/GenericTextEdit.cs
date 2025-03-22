namespace DimonSmart.StringDiff;

public class GenericTextEdit<T>
{
    public int StartPosition { get; }
    public IReadOnlyList<T> DeletedTokens { get; }
    public IReadOnlyList<T> InsertedTokens { get; }
    internal ReadOnlyMemory<T> SourceTokens { get; }

    internal GenericTextEdit(int startPosition, ReadOnlyMemory<T> deletedTokens, ReadOnlyMemory<T> insertedTokens, ReadOnlyMemory<T> sourceTokens)
    {
        StartPosition = startPosition;
        DeletedTokens = deletedTokens.ToArray();
        InsertedTokens = insertedTokens.ToArray();
        SourceTokens = sourceTokens;
    }
}