namespace DimonSmart.StringDiff;

public readonly struct GenericTextEdit<T>
{
    public int StartPosition { get; }
    public ReadOnlyMemory<T> DeletedTokens { get; }
    public ReadOnlyMemory<T> InsertedTokens { get; }
    internal ReadOnlyMemory<T> SourceTokens { get; }

    internal GenericTextEdit(int startPosition, ReadOnlyMemory<T> deletedTokens, ReadOnlyMemory<T> insertedTokens, ReadOnlyMemory<T> sourceTokens)
    {
        StartPosition = startPosition;
        DeletedTokens = deletedTokens;
        InsertedTokens = insertedTokens;
        SourceTokens = sourceTokens;
    }
}