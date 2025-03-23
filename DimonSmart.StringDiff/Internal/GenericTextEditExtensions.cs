using System.Text;

namespace DimonSmart.StringDiff.Internal;

internal static class GenericTextEditExtensions
{
    internal static TextEdit ToStringEdit<T>(this GenericTextEdit<T> edit)
    {
        var offset = (edit.StartPosition > 0 && !edit.SourceTokens.IsEmpty)
            ? SumTokenLengths(edit.SourceTokens.Span.Slice(0, edit.StartPosition))
            : 0;

        var deletedLength = (!edit.DeletedTokens.IsEmpty)
            ? SumTokenLengths(edit.DeletedTokens.Span)
            : 0;

        var insertedText = (!edit.InsertedTokens.IsEmpty)
            ? ConcatTokenStrings(edit.InsertedTokens.Span)
            : string.Empty;

        return new TextEdit(offset, deletedLength, insertedText);
    }

    private static int SumTokenLengths<T>(ReadOnlySpan<T> tokens)
    {
        var sum = 0;
        for (var i = 0; i < tokens.Length; i++)
        {
            var tokenStr = tokens[i]?.ToString();
            sum += tokenStr?.Length ?? 0;
        }
        return sum;
    }

    private static string ConcatTokenStrings<T>(ReadOnlySpan<T> tokens)
    {
        var sb = new StringBuilder();
        for (var i = 0; i < tokens.Length; i++)
        {
            sb.Append(tokens[i]?.ToString());
        }
        return sb.ToString();
    }

}