using DimonSmart.StringDiff;
using DimonSmart.StringDiff.EntityDetectors;
using Xunit;

namespace DimonSmart.StringDiffTests
{
    public class PhoneEntityDetectorTests
    {
        [Theory]
        [InlineData("+7 111 222 333", new[] { "+7 111 222 333" })]
        [InlineData("+7-111-222-333", new[] { "+7-111-222-333" })]
        [InlineData("+7111222333", new[] { "+7111222333" })]
        [InlineData("7 111 222 333", new[] { "7 111 222 333" })]
        [InlineData("Call me: +7 111 222 333", new[] { "+7 111 222 333" })]
        [InlineData("Two phones: +7 111 222 333 and +7 999 888 777", new[] { "+7 111 222 333", "+7 999 888 777" })]
        public void DetectEntities_ShouldDetectPhoneNumbers(string input, string[] expected)
        {
            var detector = new PhoneEntityDetector();
            var ranges = new Range[10];
            detector.DetectEntities(input, ranges, out var count);

            var actual = new string[count];
            for (var i = 0; i < count; i++)
            {
                actual[i] = input[ranges[i]].ToString();
            }

            Assert.Equal(expected, actual);
        }

        [Theory]
        [InlineData("")]
        [InlineData("Hello world")]
        [InlineData("123")]
        [InlineData("+7")]
        [InlineData("+7 123")]
        [InlineData("1234567")]
        public void DetectEntities_ShouldNotDetectNonPhoneNumbers(string input)
        {
            var detector = new PhoneEntityDetector();
            var ranges = new Range[10];
            detector.DetectEntities(input, ranges, out var count);
            Assert.Equal(0, count);
        }
    }
}