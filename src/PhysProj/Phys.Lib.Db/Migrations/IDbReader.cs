using Phys.Shared;

namespace Phys.Lib.Db.Migrations
{
    public interface IDbReader<T> : INamed
    {
        IDbReaderResult<T> Read(DbReaderQuery query);
    }
}
