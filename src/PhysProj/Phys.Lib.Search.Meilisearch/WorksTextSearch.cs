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

        public async Task Index(ICollection<WorkTso> values)
        {
            var task = await index.AddDocumentsAsync(values, "code");
            await TaskUtils.WaitToCompleteAsync(index, task);
            logger.LogInformation($"indexed {values.Count}");
        }

        public async Task Reset()
        {
            TaskInfo task;

            logger.LogInformation($"deleting index");
            task = (await index.DeleteAsync());
            await TaskUtils.WaitToCompleteAsync(index, task);
            logger.LogInformation($"deleted index");

            var attrs = new List<string> { "names.ru", "names.en", "authors.ru", "authors.en" };
            task = await index.UpdateSearchableAttributesAsync(attrs);
            await TaskUtils.WaitToCompleteAsync(index, task);
        }

        public async Task<ICollection<WorkTso>> Search(string search)
        {
            return (await index.SearchAsync<WorkTso>(search)).Hits.ToList();
        }
    }
}
