using Phys.Shared;

namespace Phys.Lib.Db
{
    /// <summary>
    /// Allows to enumerate all values in db in batched way
    /// </summary>
    public interface IDbReader<T> : INamed
    {
        /// <summary>
        /// Enumerates all values in db in batched way
        /// </summary>
        IEnumerable<List<T>> Read(int limit = 100);
    }
}
