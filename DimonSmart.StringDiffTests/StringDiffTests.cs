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
        [InlineData("To be or not to be", "To be or not to bee")]
        [InlineData("I have a dreem", "I have a dream")]
        [InlineData("Hello, world!", "Hello, Word!")]
        [InlineData("Edge case", "Edge cases")]
        [InlineData("", "Non-empty")]
        [InlineData("Non-empty", "")]
        [InlineData("Same text", "Same text")]
        [InlineData("1234567890", "0987654321")]
        public void ComputeDiff_ShouldRespectMinLengthOptionSpecified(string source, string target)
        {
            // Arrange
            var stringDiff = new StringDiff.StringDiff(new StringDiffOptions(10));

            // Act
            var textDiff = stringDiff.ComputeDiff(source, target);

            // Assert
            var reconstructedTarget = StringReconstructor.Instance.Reconstruct(textDiff.Edits, source);
            Assert.True(textDiff.Edits.Count <= 1);
            Assert.Equal(target, reconstructedTarget);
        }
    }
}
