using Phys.Lib.Core.Library.Models;

namespace Phys.Lib.Core.Library
{
    public interface ILibraryService
    {
        Task<SearchResultPao> Search(string? search, int limit = 16);
    }
}
