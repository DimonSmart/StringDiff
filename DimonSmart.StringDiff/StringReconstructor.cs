using System.Text;

namespace DimonSmart.StringDiff;

public class StringReconstructor
{
    public string Reconstruct(IReadOnlyCollection<TextEdit> edits, string source)
    {
        var result = new StringBuilder(source.Length * 2);
        var offset = 0;
        var currentPosition = 0;

        foreach (var edit in edits)
        {
            if (edit.StartPosition > currentPosition)
            {
                var unchangedText = source.Substring(currentPosition, edit.StartPosition - currentPosition);
                result.Append(FormatUnchangedText(unchangedText));
                currentPosition = edit.StartPosition;
            }

            var oldText = source.Substring(edit.StartPosition, edit.DeletedLength);
            var newText = edit.InsertedText;

            var formattedText = FormatText(oldText, newText);
            result.Append(formattedText);

            currentPosition = edit.StartPosition + edit.DeletedLength;
            offset += formattedText.Length - edit.DeletedLength;
        }

        // Append any remaining unchanged text after the last edit
        if (currentPosition < source.Length)
        {
            var remainingText = source.Substring(currentPosition);
            result.Append(FormatUnchangedText(remainingText));
        }

        return result.ToString();
    }

    public static StringReconstructor Instance = new();

    protected virtual string FormatInsertedText(string text) => text;

    protected virtual string FormatDeletedText(string text) => string.Empty;

    protected virtual string FormatModifiedText(string oldText, string newText) => newText;

    protected virtual string FormatUnchangedText(string text) => text;

    private string FormatText(string oldText, string newText)
    {
        if (oldText.Length == 0 && newText.Length > 0)
        {
            return FormatInsertedText(newText);
        }
        if (oldText.Length > 0 && newText.Length == 0)
        {
            return FormatDeletedText(oldText);
        }
        if (oldText.Length > 0 && newText.Length > 0)
        {
            return FormatModifiedText(oldText, newText);
        }
        return newText;
    }
}
