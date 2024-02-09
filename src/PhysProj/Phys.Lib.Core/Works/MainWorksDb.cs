using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Phys.Lib.Core.Works.Events;
using Phys.Lib.Db;
using Phys.Lib.Db.Works;
using Phys.Shared.EventBus;

namespace Phys.Lib.Core.Works
{
    internal class MainWorksDb : MainDb<IWorksDb>, IWorksDb
    {
        private readonly IEventBus eventBus;

        public MainWorksDb(Lazy<IEnumerable<IWorksDb>> dbs, IConfiguration configuration, ILogger<MainWorksDb> log, IEventBus eventBus)
            : base(dbs, configuration, log)
        {
            this.eventBus = eventBus;
        }

        public IEnumerable<List<WorkDbo>> Read(int limit)
        {
            return db.Value.Read(limit);
        }

        public void Create(string code)
        {
            db.Value.Create(code);

            eventBus.Publish(new WorkCreatedEvent { Code = code });
        }

        public void Delete(string code)
        {
            db.Value.Delete(code);

            eventBus.Publish(new WorkDeletedEvent { Code = code });
        }

        public List<WorkDbo> Find(WorksDbQuery query)
        {
            return db.Value.Find(query);
        }

        public void Update(string code, WorkDbUpdate update)
        {
            db.Value.Update(code, update);

            eventBus.Publish(new WorkUpdatedEvent { Code = code });
        }
    }
}
