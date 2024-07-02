namespace DimonSmart.StringDiff;

public class MarkdownStringReconstructor : StringReconstructor
{
    public static new MarkdownStringReconstructor Instance = new MarkdownStringReconstructor();

    protected override string FormatInsertedText(string text) => $"**{text}**";

    protected override string FormatDeletedText(string text) => $"~~{text}~~";

    protected override string FormatModifiedText(string oldText, string newText) => $"~~{oldText}~~**{newText}**";
}
