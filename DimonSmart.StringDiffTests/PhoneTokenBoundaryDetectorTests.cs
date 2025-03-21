using DimonSmart.StringDiff;
using Xunit;

namespace DimonSmart.StringDiffTests
{
    public class PhoneTokenBoundaryDetectorTests
    {
        [Theory]
        [InlineData("+7 111 222 333", new[] { "+7 111 222 333" })]
        [InlineData("+7-111-222-333", new[] { "+7-111-222-333" })]
        [InlineData("+7111222333", new[] { "+7111222333" })]
        [InlineData("7 111 222 333", new[] { "7 111 222 333" })]
        [InlineData("Call me: +7 111 222 333", new[] { "+7 111 222 333" })]
        [InlineData("Two phones: +7 111 222 333 and +7 999 888 777", new[] { "+7 111 222 333", "+7 999 888 777" })]
        public void TokenizeSpan_ShouldDetectPhoneNumbers(string input, string[] expected)
        {
            var detector = new PhoneTokenBoundaryDetector();
            var tokens = detector.TokenizeToStrings(input);
            Assert.Equal(expected, tokens);
        }

        [Theory]
        [InlineData("")]
        [InlineData("Hello world")]
        [InlineData("123")]
        [InlineData("+7")]
        [InlineData("+7 123")]
        [InlineData("1234567")]
        public void TokenizeSpan_ShouldNotDetectNonPhoneNumbers(string input)
        {
            var detector = new PhoneTokenBoundaryDetector();
            var tokens = detector.TokenizeToStrings(input);
            Assert.Empty(tokens);
        }
    }
}