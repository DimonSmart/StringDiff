namespace DimonSmart.StringDiff
{
    public class DefaultWordBoundaryDetector : IWordBoundaryDetector
    {
        public static DefaultWordBoundaryDetector Instance = new DefaultWordBoundaryDetector();

        public HashSet<int> DetectWordBeginnings(string s)
        {
            var indices = new HashSet<int>(s.Length) {};
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

        public HashSet<int> DetectWordEndings(string s)
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