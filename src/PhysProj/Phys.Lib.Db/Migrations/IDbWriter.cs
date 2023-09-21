using Phys.Shared;

namespace Phys.Lib.Db.Migrations
{
    public interface IDbWriter<in T> : INamed
    {
        void Write(IEnumerable<T> values);
    }
}
