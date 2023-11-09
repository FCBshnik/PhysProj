using Phys.Lib.Db.Authors;
using Phys.Lib.Db.Files;
using Phys.Lib.Db.Works;

namespace Phys.Lib.Core.Library.Models
{
    public class WorkModel
    {
        public required string Code { get; set; }

        public required string Name { get; set; }

        public required List<AuthorModel> Authors { get; set; }

        public required List<FileModel> Files { get; set; }

        public static WorkModel Map(WorkDbo work, IDictionary<string, AuthorDbo> authors, IDictionary<string, FileDbo> files)
        {
            return new WorkModel
            {
                Code = work.Code,
                Name = work.Infos.FirstOrDefault()?.Name ?? work.Code,
                Authors = work.AuthorsCodes.Select(a => AuthorModel.Map(authors[a])).ToList(),
                Files = work.FilesCodes.Select(a => FileModel.Map(files[a])).ToList(),
            };
        }

        public class FileModel
        {
            public required string Code { get; set; }

            public required string Format { get; set; }

            public required long Size { get; set; }

            public static FileModel Map(FileDbo file)
            {
                return new FileModel
                {
                    Code = file.Code,
                    Format = file.Format,
                    Size = file.Size,
                };
            }
        }

        public class AuthorModel
        {
            public required string Code { get; set; }

            public required string Name { get; set; }

            public static AuthorModel Map(AuthorDbo author)
            {
                return new AuthorModel
                {
                    Code = author.Code,
                    Name = author.Infos.FirstOrDefault()?.FullName ?? author.Code,
                };
            }
        }
    }
}
