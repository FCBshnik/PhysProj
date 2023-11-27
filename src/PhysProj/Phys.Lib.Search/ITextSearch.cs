namespace Phys.Lib.Search
{
    public interface ITextSearch<T>
    {
        /// <summary>
        /// Clears index
        /// </summary>
        void Reset();

        /// <summary>
        /// Adds/updates values to search engine
        /// </summary>
        void Index(ICollection<T> values);

        /// <summary>
        /// Search
        /// </summary>
        ICollection<T> Search(string search);
    }
}
