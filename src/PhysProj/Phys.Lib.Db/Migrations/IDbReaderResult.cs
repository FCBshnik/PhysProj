namespace Phys.Lib.Db.Migrations
{
    public interface IDbReaderResult<T>
    {
        string? Cursor { get; }

        IList<T> Values { get; }

        bool IsCompleted => Cursor == null;
    }
}
