using DimonSmart.StringDiff.EntityDetectors;
using Xunit;

namespace DimonSmart.StringDiffTests
{
    public class EmailEntityDetectorTests
    {
        [Theory]
        [InlineData("test@example.com", new[] { "test@example.com" })]
        [InlineData("Contact test@example.com", new[] { "test@example.com" })]
        [InlineData("My emails are: first@mail.com and second@mail.com", new[] { "first@mail.com", "second@mail.com" })]
        [InlineData("user.name@domain.co.uk", new[] { "user.name@domain.co.uk" })]
        public void DetectEntities_ShouldDetectEmails(string input, string[] expected)
        {
            var detector = new EmailEntityDetector();
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
        [InlineData("test@")]
        [InlineData("@example.com")]
        [InlineData("test.example.com")]
        [InlineData("@")]
        public void DetectEntities_ShouldNotDetectInvalidEmails(string input)
        {
            var detector = new EmailEntityDetector();
            var ranges = new Range[10];
            detector.DetectEntities(input, ranges, out var count);
            Assert.Equal(0, count);
        }
    }
}