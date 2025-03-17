namespace DimonSmart.StringDiff
{
    public class GenericTextEdit<T>
    {
        public int StartPosition { get; }
        public IReadOnlyList<T> DeletedTokens { get; }
        public IReadOnlyList<T> InsertedTokens { get; }
        private readonly IReadOnlyList<T> _sourceTokens;

        public GenericTextEdit(int startPosition, IReadOnlyList<T> deletedTokens, IReadOnlyList<T> insertedTokens, IReadOnlyList<T> sourceTokens)
        {
            StartPosition = startPosition;
            DeletedTokens = deletedTokens;
            InsertedTokens = insertedTokens;
            _sourceTokens = sourceTokens;
        }

        public TextEdit ToStringEdit()
        {
            var offset = StartPosition > 0 && _sourceTokens != null
                ? _sourceTokens.Take(StartPosition).Sum(t => t?.ToString()?.Length ?? 0)
                : 0;
            var length = DeletedTokens?.Sum(t => t?.ToString()?.Length ?? 0) ?? 0;
            var insertedText = string.Concat(InsertedTokens?.Select(t => t?.ToString() ?? "") ?? Array.Empty<string>());
            return new TextEdit(offset, length, insertedText);
        }
    }
}