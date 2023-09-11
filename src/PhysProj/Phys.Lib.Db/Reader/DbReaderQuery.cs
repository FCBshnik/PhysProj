namespace Phys.Lib.Db.Reader
{
    public class DbReaderQuery
    {
        public DbReaderQuery(int limit, string? cursor)
        {
            if (limit <= 0 || limit > 1000) throw new ArgumentOutOfRangeException(nameof(limit));

            Limit = limit;
            Cursor = cursor;
        }

        public int Limit { get; set; }

        public string? Cursor { get; set; }
    }
}
