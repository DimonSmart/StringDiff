using DimonSmart.StringDiff;
using DimonSmart.StringDiff.EntityDetectors;
using Xunit;

namespace DimonSmart.StringDiffTests
{
    public class TokenProcessorTests
    {
        [Theory]
        [InlineData(
            "Contact me at test@example.com or +7 111 222 333", 
            new[] { "Contact", " ", "me", " ", "at", " ", "test@example.com", " ", "or", " ", "+7 111 222 333" })]
        [InlineData(
            "Two contacts: first@mail.com and +7-111-222-333", 
            new[] { "Two", " ", "contacts", ": ", "first@mail.com", " ", "and", " ", "+7-111-222-333" })]
        [InlineData(
            "No entities here", 
            new[] { "No", " ", "entities", " ", "here" })]
        [InlineData("", new string[] { })]
        public void TokenizeSpan_ShouldCombineTokenizerAndEntityDetectors(string input, string[] expected)
        {
            var processor = new TokenProcessor(
                SimpleTokenizer.Instance,
                new IEntityDetector[] 
                { 
                    new EmailEntityDetector(),
                    new PhoneEntityDetector()
                });

            var ranges = new Range[100];
            processor.TokenizeSpan(input, ranges, out var count);

            var actual = new string[count];
            for (var i = 0; i < count; i++)
            {
                actual[i] = input[ranges[i]].ToString();
            }

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TokenizeSpan_ShouldPreserveEntityBoundaries()
        {
            var text = "Email test@example.com and phone +7 111 222 333 should be atomic";
            var processor = new TokenProcessor(
                SimpleTokenizer.Instance,
                new IEntityDetector[] 
                { 
                    new EmailEntityDetector(),
                    new PhoneEntityDetector()
                });

            var ranges = new Range[100];
            processor.TokenizeSpan(text, ranges, out var count);

            // Find email and phone tokens
            var emailToken = "";
            var phoneToken = "";
            for (var i = 0; i < count; i++)
            {
                var token = text[ranges[i]].ToString();
                if (token.Contains('@')) emailToken = token;
                if (token.Contains('+')) phoneToken = token;
            }

            Assert.Equal("test@example.com", emailToken);
            Assert.Equal("+7 111 222 333", phoneToken);
        }

        [Fact]
        public void TokenizeSpan_ShouldHandleOverlappingEntities()
        {
            // The first entity detector in the list should take precedence
            var text = "Contact test@phone.com";  // This could be both email and phone
            
            var processor = new TokenProcessor(
                SimpleTokenizer.Instance,
                new IEntityDetector[] 
                { 
                    new EmailEntityDetector(),  // Should win
                    new PhoneEntityDetector()
                });

            var ranges = new Range[100];
            processor.TokenizeSpan(text, ranges, out var count);

            var tokens = new string[count];
            for (var i = 0; i < count; i++)
            {
                tokens[i] = text[ranges[i]].ToString();
            }

            Assert.Contains("test@phone.com", tokens);  // Should be preserved as email
        }
    }
}