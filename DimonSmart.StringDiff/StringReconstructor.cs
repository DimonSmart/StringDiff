using System.Text;

namespace DimonSmart.StringDiff;

public static class StringReconstructor
{
    public static string Reconstruct(IReadOnlyCollection<TextEdit> edits, string source)
    {
        var result = new StringBuilder(source);
        var offset = 0;

        foreach (var edit in edits)
        {
            result.Remove(edit.StartPosition + offset, edit.DeletedLength);
            result.Insert(edit.StartPosition + offset, edit.InsertedText);
            offset += edit.InsertedText.Length - edit.DeletedLength;
        }

        return result.ToString();
    }
}

