namespace DimonSmart.StringDiff;

/// <summary>
/// Represents a text edit operation, including the start position, the length of text to be deleted, and the text to be inserted.
/// The deleted and inserted combination results in a text update or replacement.
/// </summary>
/// <param name="StartPosition">The starting position of the edit in the source text.</param>
/// <param name="DeletedLength">The length of the text to be deleted.</param>
/// <param name="InsertedText">The text to be inserted.</param>
public record TextEdit(int StartPosition, int DeletedLength, string InsertedText);
