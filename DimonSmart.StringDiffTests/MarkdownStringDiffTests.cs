using DimonSmart.StringDiff;
using Xunit;

namespace DimonSmart.StringDiffTests
{
    public class MarkdownStringDiffTests
    {
        [Theory]
        [InlineData("To be or not to be", "To be or not to bee", "To be or not to be**e**")]
        [InlineData("I have a dreem", "I have a dream", "I have a dre~~e~~**a**m")]
        [InlineData("Hello, world!", "Hello, Word!", "Hello, ~~w~~**W**or~~l~~d!")]
        [InlineData("Edge case", "Edge cases", "Edge case**s**")]
        [InlineData("", "Non-empty", "**Non-empty**")]
        [InlineData("Non-empty", "", "~~Non-empty~~")]
        [InlineData("Same text", "Same text", "Same text")]
        public void ComputeDiff_WithCustomStringReconstructor_ShouldReconstructTargetCorrectly(string source, string target, string expected)
        {
            // Arrange
            var stringDiff = new StringDiff.StringDiff();

            // Act
            var textDiff = stringDiff.ComputeDiff(source, target);

            // Assert
            var reconstructedTarget = new MarkdownStringReconstructor().Reconstruct(textDiff.Edits, source);
            Assert.Equal(expected, reconstructedTarget);
        }
    }
}
