using System;
using System.Collections.Generic;

namespace DimonSmart.StringDiff;

internal class WordDiff
{
    private const int StackAllocThreshold = 256;
    public ITokenBoundaryDetector Tokenizer { get; }
    private ReadOnlyMemory<string> SourceTokensMemory { get; set; }

    public WordDiff(ITokenBoundaryDetector tokenizer)
    {
        Tokenizer = tokenizer;
        SourceTokensMemory = Array.Empty<string>();
    }

    public IReadOnlyCollection<GenericTextEdit<string>> ComputeDiff(string sourceText, string targetText)
    {
        var sourceSpan = sourceText.AsSpan();
        var targetSpan = targetText.AsSpan();
        
        // Use stackalloc for small inputs
        var useStack = sourceSpan.Length <= StackAllocThreshold;

        var sourceRanges = useStack ? stackalloc Range[sourceSpan.Length] : new Range[sourceSpan.Length];
        var targetRanges = useStack ? stackalloc Range[targetSpan.Length] : new Range[targetSpan.Length];

        Tokenizer.TokenizeSpan(sourceSpan, sourceRanges, out var sourceTokenCount);
        Tokenizer.TokenizeSpan(targetSpan, targetRanges, out var targetTokenCount);

        // Convert token ranges to string array - now type safe
        var sourceTokens = new string[sourceTokenCount];
        var targetTokens = new string[targetTokenCount];

        for (var i = 0; i < sourceTokenCount; i++)
        {
            sourceTokens[i] = sourceSpan[sourceRanges[i]].ToString();
        }
        for (var i = 0; i < targetTokenCount; i++)
        {
            targetTokens[i] = targetSpan[targetRanges[i]].ToString();
        }

        SourceTokensMemory = sourceTokens;
        var spanEdits = new List<GenericTextEditSpan<string>>();
        DiffSpan(sourceTokens.AsMemory(), targetTokens.AsMemory(), 0, spanEdits);

        if (!useStack)
        {
            sourceRanges = default;
            targetRanges = default;
        }

        return spanEdits.Select(e => e.ToGenericTextEdit()).ToList();
    }

    private void DiffSpan(
        ReadOnlyMemory<string> source,
        ReadOnlyMemory<string> target,
        int offset,
        List<GenericTextEditSpan<string>> edits)
    {
        if (source.Length == 0 && target.Length == 0) return;

        if (source.Length == 0)
        {
            edits.Add(new GenericTextEditSpan<string>(
                offset,
                ReadOnlyMemory<string>.Empty,
                target,
                SourceTokensMemory));
            return;
        }

        if (target.Length == 0)
        {
            edits.Add(new GenericTextEditSpan<string>(
                offset,
                source,
                ReadOnlyMemory<string>.Empty,
                SourceTokensMemory));
            return;
        }

        var common = GetLongestCommonSubstring(source.Span, target.Span);

        if (common.Length == 0)
        {
            edits.Add(new GenericTextEditSpan<string>(
                offset,
                source,
                target,
                SourceTokensMemory));
            return;
        }

        // Process the part before the common substring
        DiffSpan(
            source[..common.SourceStartIndex],
            target[..common.TargetStartIndex],
            offset,
            edits);

        // Process the part after the common substring
        DiffSpan(
            source[(common.SourceStartIndex + common.Length)..],
            target[(common.TargetStartIndex + common.Length)..],
            offset + common.SourceStartIndex + common.Length,
            edits);
    }

    private static TokenSequenceMatcher.SubstringDescription GetLongestCommonSubstring(
        ReadOnlySpan<string> source,
        ReadOnlySpan<string> target)
    {
        var maxLength = 0;
        var sourceStart = 0;
        var targetStart = 0;

        // For each possible starting position in source
        for (var i = 0; i < source.Length; i++)
        {
            // For each possible starting position in target
            for (var j = 0; j < target.Length; j++)
            {
                // Try to find the longest common sequence starting at these positions
                var length = 0;
                while (i + length < source.Length && 
                       j + length < target.Length &&
                       source[i + length] == target[j + length])
                {
                    length++;
                }

                // Update if we found a longer sequence
                if (length > maxLength)
                {
                    maxLength = length;
                    sourceStart = i;
                    targetStart = j;
                }
            }
        }

        return new TokenSequenceMatcher.SubstringDescription(sourceStart, targetStart, maxLength);
    }
}