using Phys.Lib.Db.Authors;
using Phys.Lib.Db.Files;
using Phys.Lib.Db.Works;

namespace Phys.Lib.Core.Library.Models
{
    public class WorkPao : IPublicApiObject
    {
        public required string Code { get; set; }

        public required string Name { get; set; }

        public required List<AuthorPao> Authors { get; set; }

        public required List<FilePao> Files { get; set; }

        public static WorkPao Map(WorkDbo work, IDictionary<string, AuthorDbo> authors, IDictionary<string, FileDbo> files)
        {
            return new WorkPao
            {
                Code = work.Code,
                Name = work.Infos.FirstOrDefault()?.Name ?? work.Code,
                Authors = work.AuthorsCodes.Select(a => AuthorPao.Map(authors[a])).ToList(),
                Files = work.FilesCodes.Select(a => FilePao.Map(files[a])).ToList(),
            };
        }
    }
}
