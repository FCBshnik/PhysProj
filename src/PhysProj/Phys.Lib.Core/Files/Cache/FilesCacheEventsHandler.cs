using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Phys.Lib.Core.Files.Events;
using Phys.Lib.Db.Files;
using Phys.Shared.EventBus;

namespace Phys.Lib.Core.Files.Cache
{
    public class FilesCacheEventsHandler :
        IEventHandler<FilesCacheInvalidatedEvent>,
        IEventHandler<FileUpdatedEvent>,
        IHostedService
    {
        private readonly ILogger<FilesCacheEventsHandler> log;
        private readonly IFilesDb db;
        private readonly IFilesCache cache;

        public FilesCacheEventsHandler(ILogger<FilesCacheEventsHandler> log, IFilesDb db, IFilesCache cache)
        {
            this.log = log;
            this.db = db;
            this.cache = cache;
        }

        string IEventHandler<FilesCacheInvalidatedEvent>.EventName => EventNames.FilesCacheInvalidated;

        void IEventHandler<FilesCacheInvalidatedEvent>.Handle(FilesCacheInvalidatedEvent data)
        {
            PopulateCache();
        }

        string IEventHandler<FileUpdatedEvent>.EventName => EventNames.FileUpdated;

        void IEventHandler<FileUpdatedEvent>.Handle(FileUpdatedEvent data)
        {
            var file = db.GetByCode(data.Code);
            cache.Set(file);
        }

        private void PopulateCache()
        {
            foreach (var files in db.Read())
            {
                foreach (var file in files)
                {
                    cache.Set(file);
                }

                log.LogInformation($"cached {files.Count} files");
            }
        }

        Task IHostedService.StartAsync(CancellationToken cancellationToken)
        {
            return Task.Factory.StartNew(PopulateCache);
        }

        Task IHostedService.StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }
    }
}
