using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;

namespace DimonSmart.StringDiff.Benchmarks;

[MemoryDiagnoser]
public class StringDiffBenchmarks
{
    private readonly string _loremIpsum1KB;
    private readonly string _loremIpsum1KBWith10PercentChanges;
    private readonly string _completelyDifferentText1KB;
    private readonly StringDiff _stringDiff;
    private readonly StringDiff _stringDiffWithTokenizer;

    public StringDiffBenchmarks()
    {
        _loremIpsum1KB = GenerateLoremIpsum(1024);
        _loremIpsum1KBWith10PercentChanges = GenerateModifiedText(_loremIpsum1KB, 0.1);
        _completelyDifferentText1KB = GenerateCompletelyDifferentText(1024);
        _stringDiff = new StringDiff();
        _stringDiffWithTokenizer = new StringDiff(new StringDiffOptions(SimpleTokenBoundaryDetector.Instance));
    }

    [Benchmark(Description = "Identical 1KB text - Character level")]
    public TextDiff BenchmarkIdenticalText()
    {
        return _stringDiff.ComputeDiff(_loremIpsum1KB, _loremIpsum1KB);
    }

    [Benchmark(Description = "Identical 1KB text - Word level")]
    public TextDiff BenchmarkIdenticalTextWordLevel()
    {
        return _stringDiffWithTokenizer.ComputeDiff(_loremIpsum1KB, _loremIpsum1KB);
    }

    [Benchmark(Description = "10% modified 1KB text - Character level")]
    public TextDiff BenchmarkModifiedText()
    {
        return _stringDiff.ComputeDiff(_loremIpsum1KB, _loremIpsum1KBWith10PercentChanges);
    }

    [Benchmark(Description = "10% modified 1KB text - Word level")]
    public TextDiff BenchmarkModifiedTextWordLevel()
    {
        return _stringDiffWithTokenizer.ComputeDiff(_loremIpsum1KB, _loremIpsum1KBWith10PercentChanges);
    }

    [Benchmark(Description = "Completely different 1KB text - Character level")]
    public TextDiff BenchmarkDifferentText()
    {
        return _stringDiff.ComputeDiff(_loremIpsum1KB, _completelyDifferentText1KB);
    }

    [Benchmark(Description = "Completely different 1KB text - Word level")]
    public TextDiff BenchmarkDifferentTextWordLevel()
    {
        return _stringDiffWithTokenizer.ComputeDiff(_loremIpsum1KB, _completelyDifferentText1KB);
    }

    private static string GenerateLoremIpsum(int targetSize)
    {
        const string loremIpsumBase = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. ";
        var result = new System.Text.StringBuilder(targetSize);
        while (result.Length < targetSize)
        {
            result.Append(loremIpsumBase);
        }
        return result.ToString().Substring(0, targetSize);
    }

    private static string GenerateModifiedText(string source, double changeRatio)
    {
        var words = source.Split(' ');
        var random = new Random(42); // Fixed seed for reproducibility
        var wordsToChange = (int)(words.Length * changeRatio);
        
        for (var i = 0; i < wordsToChange; i++)
        {
            var idx = random.Next(words.Length);
            words[idx] = "CHANGED" + random.Next(100);
        }

        return string.Join(" ", words);
    }

    private static string GenerateCompletelyDifferentText(int size)
    {
        const string differentBase = "The quick brown fox jumps over the lazy dog. A wizard's job is to vex chumps quickly in fog. Pack my box with five dozen liquor jugs. How vexingly quick daft zebras jump! ";
        var result = new System.Text.StringBuilder(size);
        while (result.Length < size)
        {
            result.Append(differentBase);
        }
        return result.ToString().Substring(0, size);
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run<StringDiffBenchmarks>();
    }
}
