using DimonSmart.StringDiff;
using Xunit;

namespace DimonSmart.StringDiffTests
{
    public class SimpleTokenizerTests
    {
        [Theory]
        [InlineData("hello world", new[] { "hello", " ", "world" })]
        [InlineData("hello\nworld\nexample", new[] { "hello", "\n", "world", "\n", "example" })]
        [InlineData("", new string[0])]
        [InlineData("example", new[] { "example" })]
        [InlineData("abc abcde", new[] { "abc", " ", "abcde" })]
        [InlineData("abcde", new[] { "abcde" })]
        public void TokenizeSpan_ShouldReturnCorrectTokens(string input, string[] expected)
        {
            var ranges = new Range[100];
            SimpleTokenizer.Instance.TokenizeSpan(input, ranges, out var count);

            var actual = new string[count];
            for (int i = 0; i < count; i++)
            {
                actual[i] = input[ranges[i]].ToString();
            }

            Assert.Equal(expected, actual);
        }
    }
}
