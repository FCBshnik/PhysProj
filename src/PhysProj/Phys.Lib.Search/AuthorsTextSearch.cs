﻿using Meilisearch;
using Microsoft.Extensions.Logging;
using Phys.Lib.Db.Search;
using Phys.Shared.Search;

namespace Phys.Lib.Search
{
    public class AuthorsTextSearch : ITextSearch<AuthorTso>
    {
        private readonly ILogger<AuthorsTextSearch> logger;
        private readonly Meilisearch.Index index;

        public AuthorsTextSearch(Meilisearch.Index index, ILogger<AuthorsTextSearch> logger)
        {
            this.index = index;
            this.logger = logger;
        }

        public void Add(ICollection<AuthorTso> values)
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

        public ICollection<AuthorTso> Search(string search)
        {
            return index.SearchAsync<AuthorTso>(search).Result.Hits.ToList();
        }
    }
}
