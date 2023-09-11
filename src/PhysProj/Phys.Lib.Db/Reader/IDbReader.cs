namespace Phys.Lib.Db.Reader
{
    public interface IDbReader<T>
    {
        IDbReaderResult<T> Read(DbReaderQuery query);
    }
}
