namespace Phys.Shared.Search
{
    public interface ITextSearch<T>
    {
        void Reset();

        void Add(ICollection<T> values);

        ICollection<T> Search(string search);
    }
}
