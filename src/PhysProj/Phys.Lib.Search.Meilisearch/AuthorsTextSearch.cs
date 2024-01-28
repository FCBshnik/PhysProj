using Meilisearch;
using Microsoft.Extensions.Logging;
using Phys.Lib.Search.Meilisearch;

namespace Phys.Lib.Search
{
    public class AuthorsTextSearch : MeiliTextSearch<AuthorTso>
    {
        public AuthorsTextSearch(MeilisearchClient client, string name, ILogger<AuthorsTextSearch> logger)
            : base(client, name, logger, nameof(AuthorTso.Code).ToLowerInvariant())
        {
        }

        protected override IEnumerable<string> GetSearchableLanguageAttributes()
        {
            yield return nameof(AuthorTso.Names).ToLowerInvariant();
        }
    }
}
