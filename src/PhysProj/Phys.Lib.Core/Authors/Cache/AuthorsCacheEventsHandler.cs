using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Phys.Lib.Core.Works.Events;
using Phys.Lib.Db.Authors;
using Phys.Shared.EventBus;

namespace Phys.Lib.Core.Authors.Cache
{
    public class AuthorsCacheEventsHandler :
        IEventHandler<AuthorsCacheInvalidatedEvent>,
        IEventHandler<AuthorUpdatedEvent>,
        IHostedService
    {
        private readonly ILogger<AuthorsCacheEventsHandler> log;
        private readonly IAuthorsDb db;
        private readonly IAuthorsCache cache;

        public AuthorsCacheEventsHandler(ILogger<AuthorsCacheEventsHandler> log, IAuthorsDb db, IAuthorsCache cache)
        {
            this.log = log;
            this.db = db;
            this.cache = cache;
        }

        private void PopulateCache()
        {
            foreach (var authors in db.Read())
            {
                foreach (var author in authors)
                {
                    cache.Set(author);
                }

                log.LogInformation($"cached {authors.Count} authors");
            }
        }

        string IEventHandler<AuthorsCacheInvalidatedEvent>.EventName => EventNames.AuthorsCacheInvalidated;

        void IEventHandler<AuthorsCacheInvalidatedEvent>.Handle(AuthorsCacheInvalidatedEvent data)
        {
            PopulateCache();
        }

        Task IHostedService.StartAsync(CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(PopulateCache);
        }

        Task IHostedService.StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        string IEventHandler<AuthorUpdatedEvent>.EventName => EventNames.AuthorUpdated;

        void IEventHandler<AuthorUpdatedEvent>.Handle(AuthorUpdatedEvent data)
        {
            var author = db.GetByCode(data.Code);
            cache.Set(author);
        }
    }
}
