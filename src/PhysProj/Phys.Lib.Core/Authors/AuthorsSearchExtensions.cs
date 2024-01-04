using Phys.Lib.Db.Authors;
using Phys.Lib.Db.Works;
using Phys.Shared;
using Phys.Shared.Utils;

namespace Phys.Lib.Core.Authors
{
    public static class AuthorsSearchExtensions
    {
        public static Dictionary<string, AuthorDbo> GetByWorksAsMap(this IAuthorsSearch authorsSearch, IEnumerable<WorkDbo> works)
        {
            var authorsCodes = works.SelectMany(w => w.AuthorsCodes).Distinct().ToList();
            var authorsMap = authorsSearch.FindByCodes(authorsCodes).ToDictionary(a => a.Code);
            if (authorsMap.Count != authorsCodes.Count)
                throw new PhysException($"authors {authorsCodes.Except(authorsMap.Keys).Join()} not found in db");

            return authorsMap;
        }
    }
}
