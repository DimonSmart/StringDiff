namespace DimonSmart.StringDiff;

public readonly struct TextEdit(int startPosition, int deletedLength, string insertedText)
{
    public int StartPosition { get; } = startPosition;
    public int DeletedLength { get; } = deletedLength;
    public string InsertedText { get; } = insertedText;
}
