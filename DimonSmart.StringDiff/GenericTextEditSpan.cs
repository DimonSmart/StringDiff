namespace DimonSmart.StringDiff;

internal readonly struct GenericTextEditSpan<T>
{
    public int StartPosition { get; }
    public ReadOnlyMemory<T> DeletedTokens { get; }
    public ReadOnlyMemory<T> InsertedTokens { get; }
    public ReadOnlyMemory<T> SourceTokens { get; }

    public GenericTextEditSpan(int startPosition, ReadOnlyMemory<T> deletedTokens, ReadOnlyMemory<T> insertedTokens, ReadOnlyMemory<T> sourceTokens)
    {
        StartPosition = startPosition;
        DeletedTokens = deletedTokens;
        InsertedTokens = insertedTokens;
        SourceTokens = sourceTokens;
    }

    public GenericTextEdit<T> ToGenericTextEdit()
    {
        return new GenericTextEdit<T>(
            StartPosition,
            DeletedTokens.ToArray(),
            InsertedTokens.ToArray(),
            SourceTokens.ToArray());
    }
}