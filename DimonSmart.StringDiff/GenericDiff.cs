namespace DimonSmart.StringDiff
{

    public class GenericDiff<T>
    {
        public ITokenBoundaryDetector<T> Tokenizer { get; }
        public IEqualityComparer<T> Comparer { get; }
        public int MinCommonLength { get; }

        public GenericDiff(ITokenBoundaryDetector<T> tokenizer, IEqualityComparer<T>? comparer = null, int minCommonLength = 1)
        {
            Tokenizer = tokenizer;
            Comparer = comparer ?? EqualityComparer<T>.Default;
            MinCommonLength = minCommonLength;
        }

        public GenericTextDiff<T> ComputeDiff(string sourceText, string targetText)
        {
            var sourceTokens = Tokenizer.Tokenize(sourceText).ToList();
            var targetTokens = Tokenizer.Tokenize(targetText).ToList();
            var edits = Diff(sourceTokens, targetTokens, 0).ToList();
            return new GenericTextDiff<T>(sourceText, targetText, edits);
        }

        // Recursive diff algorithm using longest common substring approach
        private IEnumerable<TextEdit<T>> Diff(IList<T> sourceTokens, IList<T> targetTokens, int offset)
        {
            var edits = new List<TextEdit<T>>();

            if (sourceTokens.SequenceEqual(targetTokens, Comparer))
            {
                return edits;
            }

            if (!sourceTokens.Any() || !targetTokens.Any())
            {
                edits.Add(new TextEdit<T>(offset, sourceTokens.ToList(), targetTokens.ToList()));
                return edits;
            }

            var common = GetLongestCommonSubstring(sourceTokens, targetTokens);
            if (common.Length == 0)
            {
                edits.Add(new TextEdit<T>(offset, sourceTokens.ToList(), targetTokens.ToList()));
                return edits;
            }

            var leftSource = sourceTokens.Take(common.SourceStart).ToList();
            var leftTarget = targetTokens.Take(common.TargetStart).ToList();
            edits.AddRange(Diff(leftSource, leftTarget, offset));

            var rightSource = sourceTokens.Skip(common.SourceStart + common.Length).ToList();
            var rightTarget = targetTokens.Skip(common.TargetStart + common.Length).ToList();
            var newOffset = offset + common.SourceStart + common.Length;
            edits.AddRange(Diff(rightSource, rightTarget, newOffset));

            return edits;
        }

        // Dynamic programming implementation for longest common substring search
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
                        if (dp[i, j] > maxLen && dp[i, j] >= MinCommonLength)
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
