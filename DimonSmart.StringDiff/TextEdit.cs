namespace DimonSmart.StringDiff;

public record TextEdit(int StartPosition, int DeletedLength, string InsertedText);


// Generic text edit record
public record TextEdit<T>(int StartPosition, IReadOnlyList<T> DeletedTokens, IReadOnlyList<T> InsertedTokens);
