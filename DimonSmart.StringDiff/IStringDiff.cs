namespace DimonSmart.StringDiff;

public interface IStringDiff
{
    IReadOnlyCollection<DiffChunk> GetDiff(string oldString, string newString);
}


