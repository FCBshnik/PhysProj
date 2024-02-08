using Phys.Shared;

namespace Phys.Lib.Db
{
    /// <summary>
    /// Allows to enumerate all values in db in batched way
    /// </summary>
    public interface IDbReader<T> : INamed
    {
        IEnumerable<List<T>> Read(int limit);
    }
}
