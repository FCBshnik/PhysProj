namespace Phys.HistoryDb
{
    public interface IHistoryDbFactory
    {
        IHistoryDb<T> Create<T>(string name) where T : IHistoryDbo;
    }
}
