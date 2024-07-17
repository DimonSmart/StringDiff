using System.Text.RegularExpressions;

namespace DimonSmart.StringDiff
{
    public class DefaultWordBoundaryDetector : IWordBoundaryDetector
    {
        private readonly Regex _wordBoundaryRegex;

        public DefaultWordBoundaryDetector() : this(@"\s|\p{P}") // @"\b|\s")
        {
        }

        public DefaultWordBoundaryDetector(string regexPattern)
        {
            if (string.IsNullOrEmpty(regexPattern))
            {
                throw new ArgumentException("Regex pattern must not be null or empty.", nameof(regexPattern));
            }

            _wordBoundaryRegex = new Regex(regexPattern, RegexOptions.Compiled);
        }

        public static DefaultWordBoundaryDetector Instance = new DefaultWordBoundaryDetector();

        public HashSet<int> DetectX(string s)
        {
            var boundaries = new HashSet<int> { 0 };
            var matches = _wordBoundaryRegex.Matches(s);

            foreach (Match match in matches)
                if (match.Index < s.Length)
                    boundaries.Add(match.Index);

            if (s.Length > 0)
               boundaries.Add(s.Length - 1);

            return boundaries;
        }

        public HashSet<int> Detect(string s)
        {
            var indices = new HashSet<int>();
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

                if (inWord)
                {
                    inWord = false;
                    indices.Add(i-1);
                }

                indices.Add(i);
            }

            indices.Add(s.Length - 1);

            return indices;
        }
    }
}