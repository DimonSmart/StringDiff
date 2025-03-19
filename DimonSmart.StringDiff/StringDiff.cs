namespace DimonSmart.StringDiff
{
    public class StringDiff(StringDiffOptions? options = null) : IStringDiff
    {
        public StringDiffOptions? Options { get; } = options;

        public TextDiff ComputeDiff(string sourceText, string targetText)
        {
            if (Options?.TokenBoundaryDetector != null)
            {
                var genericDiff = new GenericDiff<string>(Options.TokenBoundaryDetector, null);
                var genericEdits = genericDiff.ComputeDiff(sourceText, targetText);
                var edits = genericEdits.Select(e => e.ToStringEdit()).ToList();
                return new TextDiff(sourceText, targetText, edits);
            }

            return new TextDiff(sourceText, targetText, Diff(sourceText.AsSpan(), targetText.AsSpan(), 0).ToList());
        }

        private IEnumerable<TextEdit> Diff(ReadOnlySpan<char> source, ReadOnlySpan<char> target, int offset)
        {
            var result = new List<TextEdit>();

            if (source.SequenceEqual(target)) return result;

            if (source.IsEmpty || target.IsEmpty)
            {
                result.Add(new TextEdit(offset, source.Length, target.ToString()));
                return result;
            }

            var common = TokenSequenceMatcher.GetLongestCommonSubstring(source, target, Options);

            if (common.Length == 0)
            {
                result.Add(new TextEdit(offset, source.Length, target.ToString()));
                return result;
            }

            result.AddRange(Diff(
                source[..common.SourceStartIndex],
                target[..common.TargetStartIndex],
                offset));
            result.AddRange(Diff(
                source[(common.SourceStartIndex + common.Length)..],
                target[(common.TargetStartIndex + common.Length)..],
                offset + common.SourceStartIndex + common.Length));

            return result;
        }
    }
}
