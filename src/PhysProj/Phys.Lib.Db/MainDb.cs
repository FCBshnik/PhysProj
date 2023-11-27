using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Phys.Shared;
using Phys.Shared.Configuration;

namespace Phys.Lib.Db
{
    public abstract class MainDb<TDbObject> where TDbObject: INamed
    {
        protected readonly Lazy<TDbObject> db;
        private readonly IConfiguration configuration;
        private readonly ILogger<MainDb<TDbObject>> log;

        protected MainDb(Lazy<IEnumerable<TDbObject>> dbs, IConfiguration configuration, ILogger<MainDb<TDbObject>> log)
        {
            this.configuration = configuration;
            this.log = log;

            db = new Lazy<TDbObject>(() => GetDb(dbs));
        }

        public string Name => "main";

        private TDbObject GetDb(Lazy<IEnumerable<TDbObject>> dbs)
        {
            var dbName = configuration.GetConnectionStringOrThrow("db");
            log.LogInformation($"use db '{dbName}' as {typeof(TDbObject)}");
            return dbs.Value.FirstOrDefault(db => db.Name == dbName && dbName != Name)
                ?? throw new PhysException($"users db '{dbName}' not found");
        }
    }
}
