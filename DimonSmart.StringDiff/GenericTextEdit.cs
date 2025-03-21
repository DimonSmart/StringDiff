namespace DimonSmart.StringDiff;

public class GenericTextEdit<T>
{
    public int StartPosition { get; }
    public ReadOnlyMemory<T> DeletedTokens { get; }
    public ReadOnlyMemory<T> InsertedTokens { get; }
    internal readonly ReadOnlyMemory<T> _sourceTokens;

    public GenericTextEdit(int startPosition, ReadOnlyMemory<T> deletedTokens, ReadOnlyMemory<T> insertedTokens, ReadOnlyMemory<T> sourceTokens)
    {
        StartPosition = startPosition;
        DeletedTokens = deletedTokens;
        InsertedTokens = insertedTokens;
        _sourceTokens = sourceTokens;
    }
}