namespace Phys.Lib.Db.Reader
{
    public interface IDbReaderResult<T>
    {
        string? Cursor { get; }

        IList<T> Values { get; }

        bool IsCompleted => Cursor == null;
    }
}
