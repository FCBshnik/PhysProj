using Phys.Shared;

namespace Phys.Lib.Db.Reader
{
    public interface IDbWriter<in T> : INamed
    {
        void Write(IEnumerable<T> values);
    }
}
