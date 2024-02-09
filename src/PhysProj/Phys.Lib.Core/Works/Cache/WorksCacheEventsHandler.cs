using Microsoft.Extensions.Logging;
using Phys.Lib.Core.Works.Events;
using Phys.Lib.Db.Works;
using Phys.Shared.Cache;
using Phys.Shared.EventBus;

namespace Phys.Lib.Core.Works.Cache
{
    public class WorksCacheEventsHandler :
        IEventHandler<WorksCacheInvalidatedEvent>,
        IEventHandler<WorkUpdatedEvent>
    {
        private readonly ILogger<WorksCacheEventsHandler> log;
        private readonly IWorksDb worksDb;
        private readonly ICache cache;

        public WorksCacheEventsHandler(ILogger<WorksCacheEventsHandler> log, IWorksDb worksDb, ICache cache)
        {
            this.log = log;
            this.worksDb = worksDb;
            this.cache = cache;
        }

        string IEventHandler<WorksCacheInvalidatedEvent>.EventName => EventNames.WorksCacheInvalidated;

        void IEventHandler<WorksCacheInvalidatedEvent>.Handle(WorksCacheInvalidatedEvent data)
        {
            foreach (var works in worksDb.Read())
            {
                foreach (var work in works)
                {
                    cache.Set(CacheKeys.Work(work.Code), work);
                }

                log.LogInformation($"cached {works.Count} works");
            }
        }

        string IEventHandler<WorkUpdatedEvent>.EventName => EventNames.WorkUpdated;

        void IEventHandler<WorkUpdatedEvent>.Handle(WorkUpdatedEvent data)
        {
            var work = worksDb.GetByCode(data.Code);
            cache.Set(CacheKeys.Work(work.Code), work);
        }
    }
}
