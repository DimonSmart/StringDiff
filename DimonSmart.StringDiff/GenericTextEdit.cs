namespace DimonSmart.StringDiff
{
    public class GenericTextEdit<T>
    {
        public int StartPosition { get; }
        public ReadOnlyMemory<T> DeletedTokens { get; }
        public ReadOnlyMemory<T> InsertedTokens { get; }
        private readonly ReadOnlyMemory<T> _sourceTokens;

        public GenericTextEdit(int startPosition, ReadOnlyMemory<T> deletedTokens, ReadOnlyMemory<T> insertedTokens, ReadOnlyMemory<T> sourceTokens)
        {
            StartPosition = startPosition;
            DeletedTokens = deletedTokens;
            InsertedTokens = insertedTokens;
            _sourceTokens = sourceTokens;
        }

        public TextEdit ToStringEdit()
        {
            var offset = StartPosition > 0 && !_sourceTokens.IsEmpty
                ? _sourceTokens.Slice(0, StartPosition).Span.ToArray().Sum(t => t?.ToString()?.Length ?? 0)
                : 0;
            var length = !DeletedTokens.IsEmpty ? DeletedTokens.Span.ToArray().Sum(t => t?.ToString()?.Length ?? 0) : 0;
            var insertedText = string.Concat(InsertedTokens.Span.ToArray().Select(t => t?.ToString() ?? ""));
            return new TextEdit(offset, length, insertedText);
        }
    }
}