using Phys.Shared.ObjectsLog;

namespace Phys.Shared.ItemsLog
{
    public interface IObjectsLog<T> where T : IObjectsLogId
    {
        void Add(T obj);

        T Get(string id);

        List<T> List(ObjectsLogQuery query);
    }
}
