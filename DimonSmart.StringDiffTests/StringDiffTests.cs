using DimonSmart.StringDiff;
using Xunit;

namespace DimonSmart.StringDiffTests
{
    public class StringDiffTests
    {
        [Theory]
        [InlineData("To be or not to be", "To be or not to bee")]
        [InlineData("I have a dreem", "I have a dream")]
        [InlineData("Hello, world!", "Hello, Word!")]
        [InlineData("Edge case", "Edge cases")]
        [InlineData("", "Non-empty")]
        [InlineData("Non-empty", "")]
        [InlineData("Same text", "Same text")]
        [InlineData("1234567890", "0987654321")]
        [InlineData("A B", "A\nB")]
        [InlineData("A\b B", "AB")]
        [InlineData("AB", "\nA\nB\n")]
        public void ComputeDiff_ShouldReconstructTargetCorrectly(string source, string target)
        {
            // Arrange
            var stringDiff = new StringDiff.StringDiff();

            // Act
            var textDiff = stringDiff.ComputeDiff(source, target);

            // Assert
            var reconstructedTarget = StringReconstructor.Instance.Reconstruct(textDiff.Edits, source);
            Assert.Equal(target, reconstructedTarget);
        }

        [Theory]
        [InlineData("To be or not to be", "To be or not to bee", new[] { "bee" })]
        [InlineData("be", "bee", new[] { "bee" })]
        [InlineData("I have a dreem", "I have a dream", new[] { "dream" })]
        [InlineData("Hello, world!", "Hello, Word!", new[] { "Word" })]
        [InlineData("Edge case", "Edge cases", new[] { "cases" })]
        [InlineData("", "Non-empty", new[] { "Non-empty" })]
        [InlineData("Non-empty", "", new[] { "" })]
        [InlineData("Same text", "Same text", new string[] { })]
        [InlineData("1234567890", "0987654321", new[] { "0987654321" })]
        [InlineData("miles to go", "kilometers to go", new[] { "kilometers" })]

        public void ComputeDiff_ShouldRespectWordBoundariesOptionSpecified(string source, string target, string[] expectedEditTexts)
        {
            // Arrange
            var stringDiff = new StringDiff.StringDiff(new StringDiffOptions(SimpleTokenizer.Instance));

            // Act
            var textDiff = stringDiff.ComputeDiff(source, target);

            // Assert
            var reconstructedTarget = StringReconstructor.Instance.Reconstruct(textDiff.Edits, source);
            Assert.True(textDiff.Edits.Count <= 1);
            Assert.Equal(target, reconstructedTarget);
            Assert.Equal(expectedEditTexts, textDiff.Edits.Select(edit => edit.InsertedText));
        }
    }
}
