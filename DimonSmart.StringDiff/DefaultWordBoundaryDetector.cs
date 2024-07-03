using System.ComponentModel;
using System.Text.RegularExpressions;

namespace DimonSmart.StringDiff
{
    public class DefaultWordBoundaryDetector : IWordBoundaryDetector
    {
        private readonly Regex _wordBoundaryRegex;

        public DefaultWordBoundaryDetector() : this(@"\b")
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

        public HashSet<int> Detect(string s)
        {
            var boundaries = new HashSet<int> { 0 };
            var matches = _wordBoundaryRegex.Matches(s);

            foreach (Match match in matches)
                boundaries.Add(match.Index);

            boundaries.Add(s.Length);

            return boundaries;
        }
    }
}