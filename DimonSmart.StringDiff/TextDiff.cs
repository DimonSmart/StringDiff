namespace DimonSmart.StringDiff;

/// <summary>
/// Represents the result of a text diff operation, including the source text, target text, and a list of text edits.
/// </summary>
/// <param name="SourceText">The original text before the edits.</param>
/// <param name="TargetText">The text after the edits.</param>
/// <param name="Edits">A list of <see cref="TextEdit"/> objects representing the differences between the source and target texts, including parts to insert, update, or delete.</param>
public record TextDiff(string SourceText, string TargetText, IReadOnlyCollection<TextEdit> Edits);
