using DimonSmart.StringDiff;
using Xunit;

namespace DimonSmart.StringDiffTests
{
    public class DefaultTokenBoundaryDetectorTests
    {
        [Theory]
        [InlineData("hello world", new[] { "hello", " ", "world" })]
        [InlineData("hello\nworld\nexample", new[] { "hello", "\n", "world", "\n", "example" })]
        [InlineData("", new string[0])]
        [InlineData("example", new[] { "example" })]
        [InlineData("abc abcde", new[] { "abc", " ", "abcde" })]
        [InlineData("abcde", new[] { "abcde" })]
        public void Tokenize_ShouldReturnCorrectTokens(string input, string[] expected)
        {
            var tokens = DefaultTokenBoundaryDetector.Instance.TokenizeToStrings(input);
            Assert.Equal(expected, tokens);
        }
    }
}
