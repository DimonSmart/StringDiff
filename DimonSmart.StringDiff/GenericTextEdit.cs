namespace DimonSmart.StringDiff
{
    public class GenericTextEdit<T>(int startPosition, IReadOnlyList<T> deletedTokens, IReadOnlyList<T> insertedTokens, IReadOnlyList<T> sourceTokens)
    {
        public int StartPosition { get; } = startPosition;
        public IReadOnlyList<T> DeletedTokens { get; } = deletedTokens;
        public IReadOnlyList<T> InsertedTokens { get; } = insertedTokens;
        private readonly IReadOnlyList<T> _sourceTokens = sourceTokens;

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