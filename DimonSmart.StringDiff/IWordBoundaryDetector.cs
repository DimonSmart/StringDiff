namespace DimonSmart.StringDiff
{
    public interface IWordBoundaryDetector
    {
        HashSet<int> Detect(string s);
    }
}