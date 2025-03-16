using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Xunit;
using DimonSmart.StringDiff;

namespace DimonSmart.StringDiff.Tests
{
    public class GenericDiffTests
    {
        // Tokenizer splits text into tokens using a regex that preserves words and non-word tokens.
        private class RegexWordBoundaryDetector : ITokenBoundaryDetector<string>
        {
            public IEnumerable<string> Tokenize(string text)
            {
                var matches = Regex.Matches(text, @"\w+|\W+");
                foreach (Match match in matches)
                {
                    yield return match.Value;
                }
            }
        }

        // Reconstructor for generic diff which rebuilds the target string from token-level edits.
        private class GenericStringReconstructor
        {
            public string Reconstruct(IReadOnlyCollection<TextEdit<string>> edits, string source, ITokenBoundaryDetector<string> tokenizer)
            {
                var tokens = tokenizer.Tokenize(source).ToList();
                var resultTokens = new List<string>();
                int currentIndex = 0;
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
        public void ComputeGenericDiff_ShouldRespectMinLengthOptionSpecified(string source, string target)
        {
            // Arrange
            var tokenizer = new RegexWordBoundaryDetector();
            var genericDiff = new GenericDiff<string>(tokenizer, minCommonLength: 10);
            var reconstructor = new GenericStringReconstructor();

            // Act
            var genericTextDiff = genericDiff.ComputeDiff(source, target);
            var reconstructedTarget = reconstructor.Reconstruct(genericTextDiff.Edits, source, tokenizer);

            // Assert
            Assert.True(genericTextDiff.Edits.Count <= 1);
            Assert.Equal(target, reconstructedTarget);
        }
    }
}
