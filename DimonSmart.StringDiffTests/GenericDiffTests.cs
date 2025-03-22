using DimonSmart.StringDiff;
using System.Text.RegularExpressions;
using Xunit;

namespace DimonSmart.StringDiffTests
{
    public class GenericDiffTests
    {
        private class RegexTokenizer : ITokenizer
        {
            public void TokenizeSpan(ReadOnlySpan<char> text, Span<Range> tokenRanges, out int tokenCount)
            {
                // Convert to string since Regex doesn't work with Span<char>
                var str = text.ToString();
                var matches = Regex.Matches(str, @"\w+|\W+");
                tokenCount = 0;

                foreach (Match match in matches)
                {
                    if (tokenCount < tokenRanges.Length)
                    {
                        tokenRanges[tokenCount++] = new Range(match.Index, match.Index + match.Length);
                    }
                }
            }
        }

        public static string Reconstruct(IReadOnlyCollection<TextEdit> edits, string source)
        {
            var result = StringReconstructor.Instance.Reconstruct(edits, source);
            return result;
        }

        [Theory]
        [InlineData("To be or not to be", "To be or not to bee")]
        [InlineData("I have a dreem", "I have a dream")]
        [InlineData("Hello, world!", "Hello, Word!")]
        [InlineData("Edge case", "Edge cases")]
        [InlineData("", "Non-empty")]
        [InlineData("Non-empty", "")]
        [InlineData("Same text", "Same text")]
        [InlineData("1234567890", "0987654321")]
        public void ComputeDiff_ShouldRespectCharacterBoundaries(string source, string target)
        {
            var tokenizer = new RegexTokenizer();
            var stringDiff = new StringDiff.StringDiff(new StringDiffOptions(tokenizer));
            var textDiff = stringDiff.ComputeDiff(source, target);
            var reconstructed = Reconstruct(textDiff.Edits, source);
            Assert.Equal(target, reconstructed);
        }
    }
}
