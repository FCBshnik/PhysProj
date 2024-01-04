using Phys.Lib.Db.Files;
using Phys.Lib.Db.Works;
using Phys.Shared;
using Phys.Shared.Utils;

namespace Phys.Lib.Core.Files
{
    public static class FilesSearchExtensions
    {
        public static IDictionary<string, FileDbo> GetByWorksAsMap(this IFilesSearch filesSearch, IEnumerable<WorkDbo> works)
        {
            var filesCodes = works.SelectMany(w => w.FilesCodes).Distinct().ToList();
            var filesMap = filesSearch.FindByCodes(filesCodes).ToDictionary(a => a.Code);
            if (filesMap.Count != filesCodes.Count)
                throw new PhysException($"files {filesCodes.Except(filesMap.Keys).Join()} not found in db");

            return filesMap;
        }
    }
}
