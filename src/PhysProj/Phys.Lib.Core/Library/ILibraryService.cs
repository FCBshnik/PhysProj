using Phys.Lib.Core.Library.Models;

namespace Phys.Lib.Core.Library
{
    public interface ILibraryService
    {
        List<WorkModel> SearchWorks(string? search);
    }
}
