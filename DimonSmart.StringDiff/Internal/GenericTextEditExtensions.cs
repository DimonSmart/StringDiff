namespace DimonSmart.StringDiff.Internal;

internal static class GenericTextEditExtensions
{
    internal static TextEdit ToStringEdit<T>(this GenericTextEdit<T> edit)
    {
        var offset = edit.StartPosition > 0 && edit.SourceTokens.Length > 0
            ? edit.SourceTokens.Slice(0, edit.StartPosition).Span.ToArray().Sum(t => t?.ToString()?.Length ?? 0)
            : 0;
        var length = edit.DeletedTokens.Count > 0 ? edit.DeletedTokens.Sum(t => t?.ToString()?.Length ?? 0) : 0;
        var insertedText = string.Concat(edit.InsertedTokens.Select(t => t?.ToString() ?? ""));
        return new TextEdit(offset, length, insertedText);
    }
}