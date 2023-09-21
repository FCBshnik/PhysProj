namespace Phys.Lib.Db.Migrations
{
    public class DbReaderResult<T> : IDbReaderResult<T>
    {
        public IList<T> Values { get; set; }

        public string? Cursor { get; set; }

        public DbReaderResult(IList<T> values, string? cursor)
        {
            ArgumentNullException.ThrowIfNull(values);

            Values = values;
            Cursor = cursor;
        }
    }
}
