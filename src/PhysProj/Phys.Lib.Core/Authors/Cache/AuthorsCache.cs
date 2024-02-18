using Phys.Lib.Db.Authors;
using Phys.Shared;
using Phys.Shared.Cache;
using Phys.Shared.Utils;

namespace Phys.Lib.Core.Authors.Cache
{
    internal class AuthorsCache : IAuthorsCache
    {
        private readonly ICache cache;

        public AuthorsCache(ICache cache)
        {
            this.cache = cache;
        }

        public List<AuthorDbo> GetAuthors(IEnumerable<string> codes)
        {
            var codesDistinct = codes.Distinct().ToList();

            var authors = codesDistinct
                .Select(c => cache.Get<AuthorDbo>(CacheKeys.Author(c)))
                .Where(w => w is not null)
                .Select(w => w!)
                .ToList();
            if (codesDistinct.Count != authors.Count)
                throw new PhysException($"authors {codesDistinct.Except(authors.Select(w => w.Code)).Join(",")} not found in cache");

            return authors;
        }

        public void Set(AuthorDbo author)
        {
            cache.Set(CacheKeys.Author(author.Code), author);
        }
    }
}
