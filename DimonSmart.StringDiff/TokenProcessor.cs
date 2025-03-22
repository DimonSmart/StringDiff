namespace DimonSmart.StringDiff;

public class TokenProcessor : ITokenizer
{
    private readonly ITokenizer _baseTokenizer;
    private readonly IEnumerable<IEntityDetector> _entityDetectors;

    public TokenProcessor(ITokenizer baseTokenizer, IEnumerable<IEntityDetector> entityDetectors)
    {
        _baseTokenizer = baseTokenizer;
        _entityDetectors = entityDetectors;
    }

    public void TokenizeSpan(ReadOnlySpan<char> text, Span<Range> tokenRanges, out int tokenCount)
    {
        if (text.IsEmpty)
        {
            tokenCount = 0;
            return;
        }

        // First, collect all entity ranges
        var claimed = new bool[text.Length];
        var allTokens = new List<Range>();
        Span<Range> tempBuffer = stackalloc Range[text.Length];

        // Detect entities first
        foreach (var detector in _entityDetectors)
        {
            detector.DetectEntities(text, tempBuffer, out var count);

            for (var i = 0; i < count; i++)
            {
                var range = tempBuffer[i];
                // Check if this range overlaps with any claimed region
                var isClean = true;
                for (var j = range.Start.Value; j < range.End.Value; j++)
                {
                    if (claimed[j])
                    {
                        isClean = false;
                        break;
                    }
                }

                if (isClean)
                {
                    // Claim this region
                    for (var j = range.Start.Value; j < range.End.Value; j++)
                    {
                        claimed[j] = true;
                    }
                    allTokens.Add(range);
                }
            }
        }

        // Process unclaimed regions with base tokenizer
        var start = 0;
        while (start < text.Length)
        {
            // Find next unclaimed region
            while (start < text.Length && claimed[start])
                start++;

            if (start >= text.Length)
                break;

            // Find end of unclaimed region
            var end = start + 1;
            while (end < text.Length && !claimed[end])
                end++;

            // Process unclaimed region
            var slice = text.Slice(start, end - start);
            _baseTokenizer.TokenizeSpan(slice, tempBuffer, out var count);

            // Add base tokens with adjusted indices
            for (var i = 0; i < count; i++)
            {
                var token = tempBuffer[i];
                allTokens.Add(new Range(
                    token.Start.Value + start,
                    token.End.Value + start));
            }

            start = end;
        }

        // Sort final tokens by position
        allTokens.Sort((a, b) => a.Start.Value.CompareTo(b.Start.Value));

        // Copy to output buffer
        tokenCount = allTokens.Count;
        for (var i = 0; i < tokenCount; i++)
        {
            tokenRanges[i] = allTokens[i];
        }
    }
}