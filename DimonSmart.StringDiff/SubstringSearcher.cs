namespace DimonSmart.StringDiff;

public static class SubstringSearcher
{
    public record SubstringResult(int SourceStartIndex, int DestinationStartIndex, int Length);

    public static SubstringResult LongestCommonSubstring(string sourceString, string destinationString)
    {
        int[,] dp = new int[sourceString.Length + 1, destinationString.Length + 1];
        int maxLength = 0;
        int sourceEndAt = 0;
        int destinationStartIndex = 0;

        for (int i = 1; i <= sourceString.Length; i++)
        {
            for (int j = 1; j <= destinationString.Length; j++)
            {
                if (sourceString[i - 1] == destinationString[j - 1])
                {
                    dp[i, j] = dp[i - 1, j - 1] + 1;
                    if (dp[i, j] > maxLength)
                    {
                        maxLength = dp[i, j];
                        sourceEndAt = i;
                        destinationStartIndex = j - maxLength;
                    }
                }
            }
        }

        return new SubstringResult(sourceEndAt - maxLength, destinationStartIndex, maxLength);
    }
}
