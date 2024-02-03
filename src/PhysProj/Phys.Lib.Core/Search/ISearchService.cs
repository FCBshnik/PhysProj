namespace Phys.Lib.Core.Search
{
    public interface ISearchService
    {
        Task<SearchWorksResult> SearchWorks(string? search, int limit = 16);
    }
}
