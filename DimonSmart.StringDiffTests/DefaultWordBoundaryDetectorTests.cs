using DimonSmart.StringDiff;
using Xunit;

namespace DimonSmart.StringDiffTests
{
    public class DefaultWordBoundaryDetectorTests
    {
        [Theory]
        [InlineData("hello world", new[] { 0, 5, 6 })]
        [InlineData("hello\nworld\nexample", new[] { 0, 5, 6, 11, 12 })]
        [InlineData("", new int [0])]
        [InlineData("example", new[] { 0 })]
        [InlineData("abc abcde", new[] { 0, 3, 4 })]
        [InlineData("abcde", new[] { 0 })]
        public void DetectWordBeginnings_ShouldReturnCorrectBoundaries(string input, int[] expected)
        {
            var boundaries = DefaultWordBoundaryDetector.Instance.DetectWordBeginnings(input);
            Assert.Equal(new HashSet<int>(expected), boundaries);
        }

        [Theory]
        [InlineData("hello world", new[] { 4, 5, 10 })]
        [InlineData("hello\nworld\nexample", new[] { 4, 5, 10, 11, 18 })]
        [InlineData("", new int[0])]
        [InlineData("example", new[] { 6 })]
        [InlineData("abc abcde", new[] { 2, 3, 8 })]
        [InlineData("abcde", new[] { 4 })]
        public void DetectWordEndings_ShouldReturnCorrectBoundaries(string input, int[] expected)
        {
            var boundaries = DefaultWordBoundaryDetector.Instance.DetectWordEndings(input);
            Assert.Equal(new HashSet<int>(expected), boundaries);
        }
    }
}
