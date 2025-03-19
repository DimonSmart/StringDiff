using System.Buffers;

namespace DimonSmart.StringDiff;

internal static class MatrixPool
{
    public static MatrixLease Rent(int rows, int cols)
    {
        var array = ArrayPool<int>.Shared.Rent((rows + 1) * (cols + 1));
        Array.Clear(array);
        return new MatrixLease(array, rows + 1, cols + 1);
    }
}

internal readonly struct MatrixLease : IDisposable
{
    private readonly int[] _array;
    private readonly int _rows;
    private readonly int _cols;

    public MatrixLease(int[] array, int rows, int cols)
    {
        _array = array;
        _rows = rows;
        _cols = cols;
    }

    public Span<int> Span => _array.AsSpan(0, _rows * _cols);

    public void Dispose()
    {
        ArrayPool<int>.Shared.Return(_array);
    }
}