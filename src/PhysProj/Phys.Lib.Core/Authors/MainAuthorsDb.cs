using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Phys.Lib.Core.Works.Events;
using Phys.Lib.Db;
using Phys.Lib.Db.Authors;
using Phys.Shared.EventBus;

namespace Phys.Lib.Core.Authors
{
    internal class MainAuthorsDb : MainDb<IAuthorsDb>, IAuthorsDb
    {
        private readonly IEventBus eventBus;

        public MainAuthorsDb(Lazy<IEnumerable<IAuthorsDb>> dbs, IConfiguration configuration, ILogger<MainAuthorsDb> log, IEventBus eventBus)
            : base(dbs, configuration, log)
        {
            this.eventBus = eventBus;
        }

        public IEnumerable<List<AuthorDbo>> Read(int limit)
        {
            return db.Value.Read(limit);
        }

        public void Create(string code)
        {
            db.Value.Create(code);

            eventBus.Publish(new AuthorCreatedEvent { Code = code });
        }

        public void Delete(string code)
        {
            db.Value.Delete(code);

            eventBus.Publish(new AuthorDeletedEvent { Code = code });
        }

        public List<AuthorDbo> Find(AuthorsDbQuery query)
        {
            return db.Value.Find(query);
        }

        public void Update(string code, AuthorDbUpdate update)
        {
            db.Value.Update(code, update);

            eventBus.Publish(new AuthorUpdatedEvent { Code = code });
        }
    }
}
