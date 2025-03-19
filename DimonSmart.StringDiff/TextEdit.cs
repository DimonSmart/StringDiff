namespace DimonSmart.StringDiff;

public readonly struct TextEdit : IEquatable<TextEdit>
{
    public int StartPosition { get; }
    public int DeletedLength { get; }
    public string InsertedText { get; }

    public TextEdit(int startPosition, int deletedLength, string insertedText)
    {
        StartPosition = startPosition;
        DeletedLength = deletedLength;
        InsertedText = insertedText;
    }

    public bool Equals(TextEdit other)
    {
        return StartPosition == other.StartPosition 
            && DeletedLength == other.DeletedLength 
            && string.Equals(InsertedText, other.InsertedText);
    }

    public override bool Equals(object? obj)
    {
        return obj is TextEdit edit && Equals(edit);
    }

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(StartPosition);
        hash.Add(DeletedLength);
        hash.Add(InsertedText);
        return hash.ToHashCode();
    }
    
    public static bool operator ==(TextEdit left, TextEdit right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(TextEdit left, TextEdit right)
    {
        return !(left == right);
    }
}
