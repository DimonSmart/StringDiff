namespace DimonSmart.StringDiff;

public record TextDiff(string SourceText, string TargetText, IReadOnlyCollection<TextEdit> Edits);

public record GenericTextDiff<T>(string SourceText, string TargetText, IReadOnlyCollection<TextEdit<T>> Edits);



