using Phys.Shared.ItemsLog;

namespace Phys.Shared.ObjectsLog
{
    public interface IObjectsLogFactory
    {
        IObjectsLog<T> Create<T>(string name) where T : IObjectsLogId;
    }
}
