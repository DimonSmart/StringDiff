namespace DimonSmart.StringDiff;

public class GenericTextEdit<T>
{
    private readonly ReadOnlyMemory<T> _deletedTokensMemory;
    private readonly ReadOnlyMemory<T> _insertedTokensMemory;
    private IReadOnlyList<T>? _deletedTokens;
    private IReadOnlyList<T>? _insertedTokens;

    public int StartPosition { get; }
    public IReadOnlyList<T> DeletedTokens => _deletedTokens ??= _deletedTokensMemory.ToArray();
    public IReadOnlyList<T> InsertedTokens => _insertedTokens ??= _insertedTokensMemory.ToArray();
    internal ReadOnlyMemory<T> SourceTokens { get; }

    internal GenericTextEdit(int startPosition, ReadOnlyMemory<T> deletedTokens, ReadOnlyMemory<T> insertedTokens, ReadOnlyMemory<T> sourceTokens)
    {
        StartPosition = startPosition;
        _deletedTokensMemory = deletedTokens;
        _insertedTokensMemory = insertedTokens;
        SourceTokens = sourceTokens;
    }
}