﻿using DimonSmart.StringDiff;
using Xunit;

namespace DimonSmart.StringDiffTests
{
    public class LongestSubstringSearcherTests
    {
        [Theory]
        [InlineData("", "", 0, 0, 0)]
        [InlineData("abc", "", 0, 0, 0)]
        [InlineData("abc", "abc", 0, 0, 3)]
        [InlineData("abc", "def", 0, 0, 0)]
        [InlineData("abcde", "cde", 2, 0, 3)]
        [InlineData("abcdeabcde", "abcde", 0, 0, 5)]
        public void LongestCommonSubstringTests(string source, string destination, int expectedSourceStartIndex, int expectedDestinationStartIndex, int expectedLength)
        {
            var result = LongestSubstringSearcher.GetLongestCommonSubstring(source, destination);
            Assert.Equal(expectedSourceStartIndex, result.SourceStartIndex);
            Assert.Equal(expectedDestinationStartIndex, result.TargetStartIndex);
            Assert.Equal(expectedLength, result.Length);
        }
    }
}