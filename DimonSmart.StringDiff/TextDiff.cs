namespace DimonSmart.StringDiff;

public record TextDiff(string SourceText, string TargetText, IReadOnlyCollection<TextEdit> Edits);

