using Meilisearch;
using Microsoft.Extensions.Logging;
using Meili = Meilisearch;

namespace Phys.Lib.Search.Meilisearch
{
    public class MeiliTextSearch<TTso> : ITextSearch<TTso>
    {
        private readonly MeilisearchClient client;
        private readonly string name;
        private readonly ILogger log;
        private readonly string primaryKey;

        private Meili.Index index;

        public MeiliTextSearch(MeilisearchClient client, string name, ILogger logger, string primaryKey)
        {
            this.client = client;
            this.name = name;
            this.log = logger;
            this.primaryKey = primaryKey;

            index = client.Index(name);

            log.LogInformation($"primary key '{primaryKey}'");
        }

        public async Task Index(IEnumerable<TTso> values)
        {
            var task = await index.AddDocumentsAsync(values, primaryKey);
            await TaskUtils.WaitToCompleteAsync(index, task);
            log.LogInformation($"indexed {values.Count()} docs");
        }

        public async Task Reset(IEnumerable<string> languages)
        {
            log.LogInformation($"resetting index '{name}'");

            TaskInfo task;

            var indexes = await client.GetAllIndexesAsync();
            if (indexes.Results.Any(i => i.Uid == index.Uid))
            {
                log.LogInformation($"deleting index");
                task = await index.DeleteAsync();
                await TaskUtils.WaitToCompleteAsync(index, task);
                log.LogInformation($"deleted index");
            }

            var langAttrs = GetSearchableLanguageAttributes();
            var attrs = langAttrs.SelectMany(attr => languages.Select(lang => $"{attr}.{lang}")).ToList();
            task = await index.UpdateSearchableAttributesAsync(attrs);
            await TaskUtils.WaitToCompleteAsync(index, task);

            log.LogInformation($"created index '{name}' with saerchable attrs {string.Join(",", attrs)}");
        }

        public async Task<ICollection<TTso>> Search(string search)
        {
            var result = await index.SearchAsync<TTso>(search);
            return result.Hits.ToList();
        }

        protected virtual IEnumerable<string> GetSearchableLanguageAttributes()
        {
            yield break;
        }
    }
}
