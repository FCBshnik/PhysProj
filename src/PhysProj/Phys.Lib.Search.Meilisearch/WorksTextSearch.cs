using Meilisearch;
using Microsoft.Extensions.Logging;
using Phys.Lib.Search.Meilisearch;

namespace Phys.Lib.Search
{
    public class WorksTextSearch : MeiliTextSearch<WorkTso>
    {
        public WorksTextSearch(MeilisearchClient client, string name, ILogger<WorksTextSearch> logger)
            :base(client, name, logger, nameof(WorkTso.Code).ToLowerInvariant())
        {
        }

        protected override IEnumerable<string> GetSearchableLanguageAttributes()
        {
            // names goes first as order has impact
            yield return nameof(WorkTso.Names).ToLowerInvariant();
            yield return nameof(WorkTso.Authors).ToLowerInvariant();
        }
    }
}
