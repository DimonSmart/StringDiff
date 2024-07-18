namespace DimonSmart.StringDiff
{
    public interface IWordBoundaryDetector
    {
        HashSet<int> DetectWordBeginnings(string s);
        HashSet<int> DetectWordEndings(string s);
    }
}