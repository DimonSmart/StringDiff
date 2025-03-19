﻿using DimonSmart.StringDiff;
using System.Text.RegularExpressions;
using Xunit;

namespace DimonSmart.StringDiffTests
{
    public class GenericDiffTests
    {
        // Tokenizer splits text into tokens using a regex that preserves words and non-word tokens.
        private class RegexTokenBoundaryDetector : ITokenBoundaryDetector
        {
            public IEnumerable<string> Tokenize(string text)
            {
                var matches = Regex.Matches(text, @"\w+|\W+");
                foreach (Match match in matches)
                {
                    yield return match.Value;
                }
            }

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

        public static string Reconstruct(IReadOnlyCollection<GenericTextEdit<string>> edits, string source, ITokenBoundaryDetector tokenizer)
        {
            var tokens = tokenizer.Tokenize(source).ToList();
            var resultTokens = new List<string>();
            var currentIndex = 0;
            // Process edits in order of increasing start position.
            foreach (var edit in edits.OrderBy(e => e.StartPosition))
            {
                while (currentIndex < edit.StartPosition && currentIndex < tokens.Count)
                {
                    resultTokens.Add(tokens[currentIndex]);
                    currentIndex++;
                }
                resultTokens.AddRange(edit.InsertedTokens);
                currentIndex += edit.DeletedTokens.Count;
            }
            while (currentIndex < tokens.Count)
            {
                resultTokens.Add(tokens[currentIndex]);
                currentIndex++;
            }
            return string.Concat(resultTokens);
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
        public void ComputeGenericDiff_ShouldRespectCharacterBoundaries(string source, string target)
        {
            // Arrange
            var tokenizer = new DefaultTokenBoundaryDetector();
            var genericDiff = new GenericDiff<string>(tokenizer);
          
            // Act
            var edits = genericDiff.ComputeDiff(source, target);
            var reconstructedTarget = Reconstruct(edits, source, tokenizer);

            // Assert
            Assert.Equal(target, reconstructedTarget);
        }
    }
}
