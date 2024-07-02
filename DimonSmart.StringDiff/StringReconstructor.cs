using System.Text;

namespace DimonSmart.StringDiff;

public class StringReconstructor
{
    public string Reconstruct(IReadOnlyCollection<TextEdit> edits, string source)
    {
        var result = new StringBuilder(source);
        var offset = 0;

        foreach (var edit in edits)
        {
            var oldText = result.ToString(edit.StartPosition + offset, edit.DeletedLength);
            var newText = edit.InsertedText;

            result.Remove(edit.StartPosition + offset, edit.DeletedLength);
            var formattedText = FormatText(oldText, newText);
            result.Insert(edit.StartPosition + offset, formattedText);

            offset += formattedText.Length - edit.DeletedLength;
        }

        return result.ToString();
    }

    public static StringReconstructor Instance = new StringReconstructor();

    protected virtual string FormatInsertedText(string text) => text;

    protected virtual string FormatDeletedText(string text) => string.Empty;

    protected virtual string FormatModifiedText(string oldText, string newText) => newText;

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
