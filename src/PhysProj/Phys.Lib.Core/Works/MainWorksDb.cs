using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Phys.Lib.Db;
using Phys.Lib.Db.Works;

namespace Phys.Lib.Core.Works
{
    internal class MainWorksDb : MainDb<IWorksDb>, IWorksDb
    {
        public MainWorksDb(Lazy<IEnumerable<IWorksDb>> dbs, IConfiguration configuration, ILogger<MainWorksDb> log)
            :base(dbs, configuration, log)
        {
        }

        public IEnumerable<List<WorkDbo>> Read(int limit)
        {
            return db.Value.Read(limit);
        }

        public void Create(string code)
        {
            db.Value.Create(code);
        }

        public void Delete(string code)
        {
            db.Value.Delete(code);
        }

        public List<WorkDbo> Find(WorksDbQuery query)
        {
            return db.Value.Find(query);
        }

        public void Update(string code, WorkDbUpdate update)
        {
            db.Value.Update(code, update);
        }
    }
}
