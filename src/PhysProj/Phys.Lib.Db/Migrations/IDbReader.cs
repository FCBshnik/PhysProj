using Phys.Shared;

namespace Phys.Lib.Db.Migrations
{
    /// <summary>
    /// Allows to enumerate all values in db in batched way
    /// </summary>
    public interface IDbReader<T> : INamed
    {
        IDbReaderResult<T> Read(DbReaderQuery query);
    }
}
