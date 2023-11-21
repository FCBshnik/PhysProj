using Meilisearch;
using Microsoft.Extensions.Logging;
using Phys.Lib.Db.Search;
using Phys.Shared.Search;

namespace Phys.Lib.Search
{
    public class WorksTextSearch : ITextSearch<WorkTso>
    {
        private readonly ILogger<WorksTextSearch> logger;
        private readonly Meilisearch.Index index;

        public WorksTextSearch(Meilisearch.Index index, ILogger<WorksTextSearch> logger)
        {
            this.index = index;
            this.logger = logger;
        }

        public void Add(ICollection<WorkTso> values)
        {
            var task = index.AddDocumentsAsync(values, "code").Result;
            TaskUtils.WaitToCompleteAsync(index, task).Wait();
            logger.LogInformation($"indexed {values.Count}");
        }

        public void Reset()
        {
            TaskInfo task;

            if (index.CreatedAt.HasValue)
            {
                logger.LogInformation($"deleting index");
                task = index.DeleteAsync().Result;
                TaskUtils.WaitToCompleteAsync(index, task).Wait();
                logger.LogInformation($"deleted index");
            }

            var attrs = new List<string> { "names.ru", "names.en" };
            task = index.UpdateSearchableAttributesAsync(attrs).Result;
            TaskUtils.WaitToCompleteAsync(index, task).Wait();
        }

        public List<WorkTso> Search(string search)
        {
            return index.SearchAsync<WorkTso>(search).Result.Hits.ToList();
        }
    }
}
