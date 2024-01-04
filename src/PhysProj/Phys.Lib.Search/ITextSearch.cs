namespace Phys.Lib.Search
{
    public interface ITextSearch<T>
    {
        /// <summary>
        /// Clears index
        /// </summary>
        Task Reset(IEnumerable<string> languages);

        /// <summary>
        /// Adds/updates values to search engine
        /// </summary>
        Task Index(IEnumerable<T> values);

        /// <summary>
        /// Search
        /// </summary>
        Task<ICollection<T>> Search(string search);
    }
}
