namespace DimonSmart.StringDiff
{
    public class GenericDiff<T>
    {
        public ITokenBoundaryDetector<T> Tokenizer { get; }
        public IEqualityComparer<T> Comparer { get; }
        public int MinCommonLength { get; }
        private IReadOnlyList<T> SourceTokens { get; set; }

        public GenericDiff(ITokenBoundaryDetector<T> tokenizer, IEqualityComparer<T>? comparer = null, int minCommonLength = 1)
        {
            Tokenizer = tokenizer;
            Comparer = comparer ?? EqualityComparer<T>.Default;
            MinCommonLength = minCommonLength;
            SourceTokens = Array.Empty<T>();
        }

        public IReadOnlyCollection<TextEdit<T>> ComputeDiff(string sourceText, string targetText)
        {
            var sourceTokens = Tokenizer.Tokenize(sourceText).ToList();
            SourceTokens = sourceTokens;
            var targetTokens = Tokenizer.Tokenize(targetText).ToList();
            return Diff(sourceTokens, targetTokens, 0).ToList();
        }

        private IEnumerable<TextEdit<T>> Diff(IList<T> sourceTokens, IList<T> targetTokens, int offset)
        {
            if (sourceTokens.SequenceEqual(targetTokens, Comparer))
                return Enumerable.Empty<TextEdit<T>>();

            if (!sourceTokens.Any())
                return new[] { new TextEdit<T>(offset, sourceTokens.ToList(), targetTokens.ToList(), SourceTokens) };

            if (!targetTokens.Any())
                return new[] { new TextEdit<T>(offset, sourceTokens.ToList(), targetTokens.ToList(), SourceTokens) };

            var common = GetLongestCommonSubstring(sourceTokens, targetTokens);
            if (common.Length == 0 || common.Length < MinCommonLength)
            {
                var deletedTokens = sourceTokens.ToList();
                var insertedTokens = targetTokens.ToList();

                // If we have both deleted and inserted tokens, try to keep punctuation/spaces
                if (deletedTokens.Any() && insertedTokens.Any())
                {
                    var lastSourceToken = deletedTokens.Last()?.ToString() ?? "";
                    var lastTargetToken = insertedTokens.Last()?.ToString() ?? "";
                    
                    if (lastSourceToken.Length > 0 && !char.IsLetterOrDigit(lastSourceToken[^1]) && lastSourceToken == lastTargetToken)
                    {
                        deletedTokens = deletedTokens.Take(deletedTokens.Count - 1).ToList();
                        insertedTokens = insertedTokens.Take(insertedTokens.Count - 1).ToList();
                    }

                    var firstSourceToken = deletedTokens.FirstOrDefault()?.ToString() ?? "";
                    var firstTargetToken = insertedTokens.FirstOrDefault()?.ToString() ?? "";
                    
                    if (deletedTokens.Any() && insertedTokens.Any() && firstSourceToken.Length > 0 &&
                        !char.IsLetterOrDigit(firstSourceToken[0]) && firstSourceToken == firstTargetToken)
                    {
                        deletedTokens = deletedTokens.Skip(1).ToList();
                        insertedTokens = insertedTokens.Skip(1).ToList();
                        offset += 1;
                    }
                }

                if (deletedTokens.Any() || insertedTokens.Any())
                {
                    return new[] { new TextEdit<T>(offset, deletedTokens, insertedTokens, SourceTokens) };
                }
                return Enumerable.Empty<TextEdit<T>>();
            }

            var edits = new List<TextEdit<T>>();

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
