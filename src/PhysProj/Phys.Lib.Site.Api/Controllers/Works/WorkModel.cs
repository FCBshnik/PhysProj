using Phys.Lib.Db.Authors;
using Phys.Lib.Db.Works;

namespace Phys.Lib.Site.Api.Controllers.Works
{
    public class WorkModel
    {
        public required string Code { get; set; }

        public required string Name { get; set; }

        public required List<AuthorModel> Authors { get; set; }

        public static WorkModel Map(WorkDbo work, IDictionary<string, AuthorDbo> authors)
        {
            return new WorkModel
            {
                Code = work.Code,
                Name = work.Infos.FirstOrDefault()?.Name ?? work.Code,
                Authors = work.AuthorsCodes.Select(a => AuthorModel.Map(authors[a])).ToList(),
            };
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
