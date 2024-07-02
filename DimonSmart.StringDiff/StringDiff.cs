namespace DimonSmart.StringDiff;

public class StringDiff : IStringDiff
{
    public IReadOnlyCollection<DiffChunk> GetDiff(string sourceString, string destinationString)
    {
        return GetDiff(sourceString, destinationString);
    }

    public IEnumerable<DiffChunk> GetDiff(string sourceString, string destinationString, int offset)
    { 
        var result = new List<DiffChunk>();
        if (sourceString == destinationString)
            yield break;

        if (sourceString == string.Empty || destinationString == string.Empty)
        {
            yield return new DiffChunk(sourceString, offset, destinationString, offset);
            yield break;
        }

        var commonPart = SubstringSearcher.LongestCommonSubstring(sourceString, destinationString);
        if (commonPart.Length == 0)
        {
            yield return new DiffChunk(sourceString, offset, destinationString, offset);
            yield break;
        }

        var left = GetDiff(sourceString.Substring(0, commonPart.SourceStartIndex), destinationString.Substring(0, commonPart.DestinationStartIndex), offset);
        foreach (var chunk in left) yield return chunk;

        var right = GetDiff(sourceString.Substring(commonPart.SourceStartIndex + commonPart.Length), destinationString.Substring(commonPart.DestinationStartIndex + commonPart.Length), offset + commonPart.SourceStartIndex + commonPart.Length);
        foreach (var chunk in right) yield return chunk;
    }
}


