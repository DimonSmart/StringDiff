using System.Runtime.CompilerServices;

namespace DimonSmart.StringDiff;

public ref struct TokenRangeList
{
    private readonly Span<Range> _ranges;
    private int _count;

    public TokenRangeList(Span<Range> ranges)
    {
        _ranges = ranges;
        _count = 0;
    }

    public ReadOnlySpan<Range> AsReadOnlySpan() => _ranges[.._count];
    
    public int Count => _count;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(Range range)
    {
        if (_count < _ranges.Length)
        {
            _ranges[_count++] = range;
        }
    }

    public Range this[int index]
    {
        get => _ranges[index];
        set => _ranges[index] = value;
    }
}