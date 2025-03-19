namespace DimonSmart.StringDiff
{
    public class GenericDiff<T>
    {
        public ITokenBoundaryDetector Tokenizer { get; }
        public IEqualityComparer<T> Comparer { get; }
        private IReadOnlyList<T> SourceTokens { get; set; }

        public GenericDiff(ITokenBoundaryDetector tokenizer, IEqualityComparer<T>? comparer = null)
        {
            Tokenizer = tokenizer;
            Comparer = comparer ?? EqualityComparer<T>.Default;
            SourceTokens = Array.Empty<T>();
        }

        public IReadOnlyCollection<GenericTextEdit<T>> ComputeDiff(string sourceText, string targetText)
        {
            var sourceTokens = Tokenizer.Tokenize(sourceText).Cast<T>().ToList();
            SourceTokens = sourceTokens;
            var targetTokens = Tokenizer.Tokenize(targetText).Cast<T>().ToList();
            return Diff(sourceTokens, targetTokens, 0).ToList();
        }

        private IEnumerable<GenericTextEdit<T>> Diff(IList<T> sourceTokens, IList<T> targetTokens, int offset)
        {
            if (sourceTokens.SequenceEqual(targetTokens, Comparer))
                return Enumerable.Empty<GenericTextEdit<T>>();

            if (!sourceTokens.Any())
                return new[] { new GenericTextEdit<T>(offset, sourceTokens.ToList(), targetTokens.ToList(), SourceTokens) };

            if (!targetTokens.Any())
                return new[] { new GenericTextEdit<T>(offset, sourceTokens.ToList(), targetTokens.ToList(), SourceTokens) };

            var common = GetLongestCommonSubstring(sourceTokens, targetTokens);
            if (common.Length == 0)
            {
                var deletedTokens = sourceTokens.ToList();
                var insertedTokens = targetTokens.ToList();

                if (deletedTokens.Any() || insertedTokens.Any())
                {
                    return new[] { new GenericTextEdit<T>(offset, deletedTokens, insertedTokens, SourceTokens) };
                }
                return Enumerable.Empty<GenericTextEdit<T>>();
            }

            var edits = new List<GenericTextEdit<T>>();

            // Handle prefix changes
            if (common.SourceStart > 0 || common.TargetStart > 0)
            {
                edits.AddRange(Diff(
                    sourceTokens.Take(common.SourceStart).ToList(),
                    targetTokens.Take(common.TargetStart).ToList(),
                    offset));
            }

            // Handle suffix changes
            var sourceRightStart = common.SourceStart + common.Length;
            var targetRightStart = common.TargetStart + common.Length;
            
            if (sourceRightStart < sourceTokens.Count || targetRightStart < targetTokens.Count)
            {
                edits.AddRange(Diff(
                    sourceTokens.Skip(sourceRightStart).ToList(),
                    targetTokens.Skip(targetRightStart).ToList(),
                    offset + sourceRightStart));
            }

            return edits;
        }

        private SubstringDescription GetLongestCommonSubstring(IList<T> source, IList<T> target)
        {
            var dp = new int[source.Count + 1, target.Count + 1];
            var maxLen = 0;
            int sourceIndex = 0, targetIndex = 0;

            for (var i = 1; i <= source.Count; i++)
            {
                for (var j = 1; j <= target.Count; j++)
                {
                    if (Comparer.Equals(source[i - 1], target[j - 1]))
                    {
                        dp[i, j] = dp[i - 1, j - 1] + 1;
                        if (dp[i, j] > maxLen)
                        {
                            maxLen = dp[i, j];
                            sourceIndex = i - maxLen;
                            targetIndex = j - maxLen;
                        }
                    }
                    else
                    {
                        dp[i, j] = 0;
                    }
                }
            }

            return new SubstringDescription(sourceIndex, targetIndex, maxLen);
        }

        private record SubstringDescription(int SourceStart, int TargetStart, int Length);
    }
}
