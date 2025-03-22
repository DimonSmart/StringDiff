using DimonSmart.StringDiff.Internal;

namespace DimonSmart.StringDiff;

public class CharDiff : IStringDiff
{
    public TextDiff ComputeDiff(string sourceText, string targetText)
    {
        var source = sourceText.AsMemory();
        var target = targetText.AsMemory();
        var edits = new List<GenericTextEditSpan<char>>();
        ComputeTextDifferences(source.Span, target.Span, 0, edits, source);
        var stringEdits = edits.Select(e => e.ToGenericTextEdit()).Select(e => e.ToStringEdit()).ToList();
        return new TextDiff(sourceText, targetText, stringEdits);
    }

    private static void ComputeTextDifferences(
        ReadOnlySpan<char> source,
        ReadOnlySpan<char> target,
        int offset,
        List<GenericTextEditSpan<char>> edits,
        ReadOnlyMemory<char> sourceMemory)
    {
        if (source.Length == 0 && target.Length == 0)
        {
            return;
        }

        if (source.Length == 0)
        {
            edits.Add(new GenericTextEditSpan<char>(offset, ReadOnlyMemory<char>.Empty, target.ToArray(), sourceMemory));
            return;
        }

        if (target.Length == 0)
        {
            edits.Add(new GenericTextEditSpan<char>(offset, source.ToArray(), ReadOnlyMemory<char>.Empty, sourceMemory));
            return;
        }

        var common = TokenSequenceMatcher.GetLongestCommonSubstring(source, target);

        if (common.Length == 0)
        {
            edits.Add(new GenericTextEditSpan<char>(offset, source.ToArray(), target.ToArray(), sourceMemory));
            return;
        }

        // Process the part before the common substring
        ComputeTextDifferences(
            source[..common.SourceStartIndex],
            target[..common.TargetStartIndex],
            offset,
            edits,
            sourceMemory);

        // Process the part after the common substring
        ComputeTextDifferences(
            source[(common.SourceStartIndex + common.Length)..],
            target[(common.TargetStartIndex + common.Length)..],
            offset + common.SourceStartIndex + common.Length,
            edits,
            sourceMemory);
    }
}
