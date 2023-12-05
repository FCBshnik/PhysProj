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

        public void Index(ICollection<WorkTso> values)
        {
            var task = index.AddDocumentsAsync(values, "code").Result;
            TaskUtils.WaitToCompleteAsync(index, task).Wait();
            logger.LogInformation($"indexed {values.Count}");
        }

        public void Reset()
        {
            TaskInfo task;

            logger.LogInformation($"deleting index");
            task = index.DeleteAsync().Result;
            TaskUtils.WaitToCompleteAsync(index, task).Wait();
            logger.LogInformation($"deleted index");

            var attrs = new List<string> { "names.ru", "names.en", "authors.ru", "authors.en" };
            task = index.UpdateSearchableAttributesAsync(attrs).Result;
            TaskUtils.WaitToCompleteAsync(index, task).Wait();
        }

        public ICollection<WorkTso> Search(string search)
        {
            return index.SearchAsync<WorkTso>(search).Result.Hits.ToList();
        }
    }
}
