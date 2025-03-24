using DimonSmart.StringDiff;
using Xunit;

namespace DimonSmart.StringDiffTests
{
    public class GenericDiffTests
    {
        public static string Reconstruct(IReadOnlyCollection<TextEdit> edits, string source)
        {
            var result = StringReconstructor.Instance.Reconstruct(edits, source);
            return result;
        }

        [Theory]
        [InlineData("To be or not to be", "To be or not to bee")]
        [InlineData("I have a dreem", "I have a dream")]
        [InlineData("Hello, world!", "Hello, Word!")]
        [InlineData("Edge case", "Edge cases")]
        [InlineData("", "Non-empty")]
        [InlineData("Non-empty", "")]
        [InlineData("Same text", "Same text")]
        [InlineData("1234567890", "0987654321")]
        public void ComputeDiff_ShouldRespectCharacterBoundaries(string source, string target)
        {
            var stringDiff = new StringDiff.StringDiff(new StringDiffOptions(SimpleTokenizer.Instance));
            var textDiff = stringDiff.ComputeDiff(source, target);
            var reconstructed = Reconstruct(textDiff.Edits, source);
            Assert.Equal(target, reconstructed);
        }
    }
}
