namespace DimonSmart.StringDiff
{
    public class DefaultTokenBoundaryDetector : ITokenBoundaryDetector
    {
        public static DefaultTokenBoundaryDetector Instance = new();

        public IEnumerable<string> Tokenize(string text)
        {
            if (string.IsNullOrEmpty(text))
                yield break;

            var i = 0;
            while (i < text.Length)
            {
                if (char.IsLetterOrDigit(text[i]))
                {
                    var start = i;
                    while (i < text.Length && char.IsLetterOrDigit(text[i]))
                    {
                        i++;
                    }
                    yield return text.Substring(start, i - start);
                }
                else
                {
                    yield return text[i].ToString();
                    i++;
                }
            }
        }
    }
}