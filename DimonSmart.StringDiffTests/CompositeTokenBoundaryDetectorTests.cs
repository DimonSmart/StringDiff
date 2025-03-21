using DimonSmart.StringDiff;
using Xunit;

namespace DimonSmart.StringDiffTests
{
    public class CompositeTokenBoundaryDetectorTests
    {
        [Theory]
        [InlineData("Contact test@example.com", new[] { "Contact", " ", "test@example.com" })]
        [InlineData("My emails are: first@mail.com and second@mail.com",
                    new[] { "My", " ", "emails", " ", "are", ": ", "first@mail.com", " ", "and", " ", "second@mail.com" })]
        [InlineData("No emails here", new[] { "No", " ", "emails", " ", "here" })]
        [InlineData("", new string[] { })]
        [InlineData(" ", new string[] { " " })]
        [InlineData("\t\n", new string[] { "\t\n" })]

        public void Tokenize_ShouldDetectEmails(string input, string[] expected)
        {
            var composite = new CompositeTokenBoundaryDetector(new ITokenBoundaryDetector[]
            {
                new EmailTokenBoundaryDetector()
            });

            var tokens = composite.TokenizeToStrings(input);
            Assert.Equal(expected, tokens);
        }
    }
}
