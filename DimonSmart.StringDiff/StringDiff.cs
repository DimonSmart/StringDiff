namespace DimonSmart.StringDiff
{
    public class StringDiff : IStringDiff
    {
        public TextDiff ComputeDiff(string sourceText, string targetText)
        {
            return new TextDiff(sourceText, targetText, Diff(sourceText, targetText, 0).ToList());
        }

        private static IEnumerable<TextEdit> Diff(string source, string target, int offset)
        {
            var result = new List<TextEdit>();

            if (source == target) return result;

            if (source.Length == 0 || target.Length == 0)
            {
                result.Add(new TextEdit(offset, source.Length, target));
                return result;
            }

            var common = SubstringSearcher.LongestCommonSubstring(source, target);

            if (common.Length == 0)
            {
                result.Add(new TextEdit(offset, source.Length, target));
                return result;
            }

            result.AddRange(Diff(
                source[..common.SourceStartIndex],
                target[..common.TargetStartIndex], offset));
            result.AddRange(Diff(
                source[(common.SourceStartIndex + common.Length)..],
                target[(common.TargetStartIndex + common.Length)..],
                offset + common.SourceStartIndex + common.Length));

            return result;
        }
    }
}
