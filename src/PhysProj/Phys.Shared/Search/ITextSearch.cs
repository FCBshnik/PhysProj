namespace Phys.Shared.Search
{
    public interface ITextSearch<T>
    {
        void Reset();

        void Add(ICollection<T> values);

        List<T> Search(string search);
    }
}
