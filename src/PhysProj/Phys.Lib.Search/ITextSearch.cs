namespace Phys.Lib.Search
{
    public interface ITextSearch<TTextSearchObject>
    {
        /// <summary>
        /// Clears index
        /// </summary>
        Task Reset(IEnumerable<string> languages);

        /// <summary>
        /// Adds/updates values to search engine
        /// </summary>
        Task Index(IEnumerable<TTextSearchObject> values);

        /// <summary>
        /// Search
        /// </summary>
        Task<ICollection<TTextSearchObject>> Search(string search);
    }
}
