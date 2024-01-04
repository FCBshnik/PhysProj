using Meilisearch;
using Microsoft.Extensions.Logging;
using Phys.Lib.Search.Meilisearch;
using Meili = Meilisearch;

namespace Phys.Lib.Search
{
    public class WorksTextSearch : ITextSearch<WorkTso>
    {
        private readonly ILogger<WorksTextSearch> logger;
        private readonly Meili.Index index;

        public WorksTextSearch(Meili.Index index, ILogger<WorksTextSearch> logger)
        {
            this.index = index;
            this.logger = logger;
        }

        public async Task Index(IEnumerable<WorkTso> values)
        {
            var task = await index.AddDocumentsAsync(values, nameof(WorkTso.Code).ToLowerInvariant());
            await TaskUtils.WaitToCompleteAsync(index, task);
            logger.LogInformation($"indexed {values.Count()}");
        }

        public async Task Reset(IEnumerable<string> languages)
        {
            TaskInfo task;

            logger.LogInformation($"deleting index");
            task = await index.DeleteAsync();
            await TaskUtils.WaitToCompleteAsync(index, task);
            logger.LogInformation($"deleted index");

            var namesAttr = nameof(WorkTso.Names).ToLowerInvariant();
            var authorsAttr = nameof(WorkTso.Authors).ToLowerInvariant();
            // names goes first as order has impact
            var attrs = languages.Select(l => $"{namesAttr}.{l}").Concat(languages.Select(l => $"{authorsAttr}.{l}")).ToList();
            task = await index.UpdateSearchableAttributesAsync(attrs);
            await TaskUtils.WaitToCompleteAsync(index, task);
        }

        public async Task<ICollection<WorkTso>> Search(string search)
        {
            return (await index.SearchAsync<WorkTso>(search)).Hits.ToList();
        }
    }
}
