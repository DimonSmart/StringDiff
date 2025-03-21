namespace DimonSmart.StringDiff;

public static class GenericTextEditExtensions
{
    public static TextEdit ToStringEdit<T>(this GenericTextEdit<T> edit)
    {
        var offset = edit.StartPosition > 0 && !edit._sourceTokens.IsEmpty
            ? edit._sourceTokens.Slice(0, edit.StartPosition).Span.ToArray().Sum(t => t?.ToString()?.Length ?? 0)
            : 0;
        var length = !edit.DeletedTokens.IsEmpty ? edit.DeletedTokens.Span.ToArray().Sum(t => t?.ToString()?.Length ?? 0) : 0;
        var insertedText = string.Concat(edit.InsertedTokens.Span.ToArray().Select(t => t?.ToString() ?? ""));
        return new TextEdit(offset, length, insertedText);
    }
}