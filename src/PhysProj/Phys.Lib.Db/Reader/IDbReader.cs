using Phys.Shared;

namespace Phys.Lib.Db.Reader
{
    public interface IDbReader<T> : INamed
    {
        IDbReaderResult<T> Read(DbReaderQuery query);
    }
}
