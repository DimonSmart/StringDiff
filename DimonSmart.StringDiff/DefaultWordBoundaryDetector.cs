namespace DimonSmart.StringDiff
{
    public class DefaultTokenBoundaryDetector : ITokenBoundaryDetector<string>
    {
        public static DefaultTokenBoundaryDetector Instance = new DefaultTokenBoundaryDetector();

        public IEnumerable<string> Tokenize(string text)
        {
            if (string.IsNullOrEmpty(text))
                return Enumerable.Empty<string>();

            var wordBeginnings = DetectWordBeginnings(text);
            var wordEndings = DetectWordEndings(text);
            
            return wordBeginnings.OrderBy(i => i)
                .Zip(wordEndings.OrderBy(i => i), (start, end) => text.Substring(start, end - start + 1));
        }

        private HashSet<int> DetectWordBeginnings(string s)
        {
            var indices = new HashSet<int>(s.Length) { };
            if (string.IsNullOrEmpty(s))
                return indices;

            var inWord = false;
            for (var i = 0; i < s.Length; i++)
            {
                if (char.IsLetterOrDigit(s[i]))
                {
                    if (!inWord)
                    {
                        indices.Add(i);
                        inWord = true;
                    }
                    continue;
                }

                inWord = false;
                indices.Add(i);
            }

            indices.Add(0);
            return indices;
        }

        private HashSet<int> DetectWordEndings(string s)
        {
            var indices = new HashSet<int>();
            if (string.IsNullOrEmpty(s))
                return indices;

            var inWord = false;
            for (var i = 0; i < s.Length; i++)
            {
                if (char.IsLetterOrDigit(s[i]))
                {
                    inWord = true;
                    continue;
                }

                if (inWord)
                {
                    inWord = false;
                    indices.Add(i - 1);
                }

                indices.Add(i);
            }

            indices.Add(s.Length - 1);
            return indices;
        }
    }
}