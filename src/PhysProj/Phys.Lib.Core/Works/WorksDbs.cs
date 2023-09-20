using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Phys.Lib.Db.Works;
using Phys.Shared;

namespace Phys.Lib.Core.Works
{
    internal class WorksDbs : IWorksDbs
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<WorksDbs> log;
        private readonly Lazy<IWorksDb> db;

        public string Name => "main";

        public WorksDbs(IEnumerable<IWorksDb> dbs, IConfiguration configuration, ILogger<WorksDbs> log)
        {
            this.configuration = configuration;
            this.log = log;

            db = new Lazy<IWorksDb>(() => GetDb(dbs));
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

        private IWorksDb GetDb(IEnumerable<IWorksDb> dbs)
        {
            var dbName = configuration.GetConnectionString("db");
            if (dbName == null)
                throw new PhysException($"connection string 'db' is missed");

            log.LogInformation($"use db '{dbName}' as {typeof(IWorksDb)}");
            return dbs.FirstOrDefault(db => db.Name == dbName && dbName != Name)
                ?? throw new PhysException($"works db '{dbName}' not found");
        }
    }
}
