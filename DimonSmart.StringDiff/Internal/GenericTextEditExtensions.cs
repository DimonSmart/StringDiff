namespace DimonSmart.StringDiff.Internal;

internal static class GenericTextEditExtensions
{
    internal static TextEdit ToStringEdit<T>(this GenericTextEdit<T> edit)
    {
        var offset = edit.StartPosition > 0 && edit.SourceTokens.Length > 0
            ? edit.SourceTokens.Slice(0, edit.StartPosition).ToArray().Sum(t => t?.ToString()?.Length ?? 0)
            : 0;
        var length = edit.DeletedTokens.Length > 0 ? edit.DeletedTokens.ToArray().Sum(t => t?.ToString()?.Length ?? 0) : 0;
        var insertedText = String.Concat(edit.InsertedTokens.ToArray().Select(t => t?.ToString() ?? ""));
        return new TextEdit(offset, length, insertedText);
    }
}