using DimonSmart.StringDiff;
using Xunit;

namespace DimonSmart.StringDiffTests
{
    public class TokenSequenceMatcherTests
    {
        [Theory]
        [InlineData("", "", 0, 0, 0)]
        [InlineData("abc", "", 0, 0, 0)]
        [InlineData("abc", "abc", 0, 0, 3)]
        [InlineData("abc", "def", 0, 0, 0)]
        [InlineData("abcde", "cde", 2, 0, 3)]
        [InlineData("abcdeabcde", "abcde", 0, 0, 5)]
        [InlineData("Hello world", "Hello World!", 0, 0, 6)]
        public void GetLongestCommonSubstringTests(string source, string destination, int expectedSourceStartIndex, int expectedDestinationStartIndex, int expectedLength)
        {
            var result = TokenSequenceMatcher.GetLongestCommonSubstring(source, destination, new StringDiffOptions());
            Assert.Equal(expectedSourceStartIndex, result.SourceStartIndex);
            Assert.Equal(expectedDestinationStartIndex, result.TargetStartIndex);
            Assert.Equal(expectedLength, result.Length);
        }

        [Theory]
        [InlineData("", "", 0, 0, 0)]
        [InlineData("abc", "", 0, 0, 0)]
        [InlineData("abc", "abc", 0, 0, 3)]
        [InlineData("abc", "def", 0, 0, 0)]
        [InlineData("abcde", "cde", 0, 0, 0)]
        [InlineData("abc abcde", "abcde", 4, 0, 5)]
        [InlineData("Hello world", "Hello World!", 0, 0, 6)]
        public void GetLongestCommonSubstringRespectTokenBoundariesTests(string source, string destination, int expectedSourceStartIndex, int expectedDestinationStartIndex, int expectedLength)
        {
            var result = TokenSequenceMatcher.GetLongestCommonSubstring(source, destination, new StringDiffOptions(SimpleTokenBoundaryDetector.Instance));
            Assert.Equal(expectedSourceStartIndex, result.SourceStartIndex);
            Assert.Equal(expectedDestinationStartIndex, result.TargetStartIndex);
            Assert.Equal(expectedLength, result.Length);
        }
    }
}
