namespace DimonSmart.StringDiff;

public interface IStringDiff
{
    TextDiff ComputeDiff(string sourceText, string targetText);
}
