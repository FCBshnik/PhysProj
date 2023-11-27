using Phys.Lib.Core.Library.Models;

namespace Phys.Lib.Core.Library
{
    public interface ILibraryService
    {
        List<WorkPao> SearchWorks(string? search);

        SearchResultPao Search(string? search);
    }
}
