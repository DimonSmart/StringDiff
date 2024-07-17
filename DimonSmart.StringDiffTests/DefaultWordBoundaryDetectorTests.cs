using DimonSmart.StringDiff;
using Xunit;

namespace DimonSmart.StringDiffTests
{
    public class DefaultWordBoundaryDetectorTests
    {
        [Theory]
        [InlineData("hello world", new[] { 0, 4, 5, 6, 10 })]
        [InlineData("hello\nworld\nexample", new[] { 0, 4, 5, 6, 10, 11, 12, 18 })]
        [InlineData("", new int [0])]
        [InlineData("example", new[] { 0, 6 })]
        [InlineData("abc abcde", new[] { 0, 2, 3, 4, 8 })]
        [InlineData("abcde", new[] { 0, 4 })]
        public void Detect_ShouldReturnCorrectBoundaries(string input, int[] expected)
        {
            var boundaries = DefaultWordBoundaryDetector.Instance.Detect(input);
            Assert.Equal(new HashSet<int>(expected), boundaries);
        }
    }
}
