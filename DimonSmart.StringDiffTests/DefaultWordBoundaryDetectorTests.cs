using DimonSmart.StringDiff;
using Xunit;

namespace DimonSmart.StringDiffTests
{
    public class DefaultWordBoundaryDetectorTests
    {
        [Theory]
        [InlineData("hello world", new[] { 0, 5, 6, 11 })]
        [InlineData("hello\nworld\nexample", new[] { 0, 5, 6, 11, 12, 19 })]
        [InlineData("", new[] { 0 })]
        [InlineData("example", new[] { 0, 7 })]
        public void Detect_ShouldReturnCorrectBoundaries(string input, int[] expected)
        {
            var boundaries = DefaultWordBoundaryDetector.Instance.Detect(input);
            Assert.Equal(new HashSet<int>(expected), boundaries);
        }
    }
}
