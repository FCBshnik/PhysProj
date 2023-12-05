using Meilisearch;
using Microsoft.Extensions.Logging;
using Phys.Lib.Search.Meilisearch;
using Meili = Meilisearch;

namespace Phys.Lib.Search
{
    public class AuthorsTextSearch : ITextSearch<AuthorTso>
    {
        private readonly ILogger<AuthorsTextSearch> logger;
        private readonly Meili.Index index;

        public AuthorsTextSearch(Meili.Index index, ILogger<AuthorsTextSearch> logger)
        {
            this.index = index;
            this.logger = logger;
        }

        public async Task Index(ICollection<AuthorTso> values)
        {
            var task = await index.AddDocumentsAsync(values, "code");
            await TaskUtils.WaitToCompleteAsync(index, task);
            logger.LogInformation($"indexed {values.Count}");
        }

        public async Task Reset()
        {
            TaskInfo task;

            logger.LogInformation($"deleting index");
            task = await index.DeleteAsync();
            await TaskUtils.WaitToCompleteAsync(index, task);
            logger.LogInformation($"deleted index");

            var attrs = new List<string> { "names.ru", "names.en" };
            task = await index.UpdateSearchableAttributesAsync(attrs);
            await TaskUtils.WaitToCompleteAsync(index, task);
        }

        public async Task<ICollection<AuthorTso>> Search(string search)
        {
            return (await index.SearchAsync<AuthorTso>(search)).Hits.ToList();
        }
    }
}
