namespace Phys.HistoryDb
{
    public interface IHistoryDb<T> where T : IHistoryDbo
    {
        void Save(T obj);

        T Get(string id);

        List<T> List(HistoryDbQuery query);
    }
}
