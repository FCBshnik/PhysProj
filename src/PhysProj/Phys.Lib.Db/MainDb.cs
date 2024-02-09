using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Phys.Shared;
using Phys.Shared.Configuration;

namespace Phys.Lib.Db
{
    /// <summary>
    /// Wrapper over all db implementations
    /// Selects main db (which will be used by all services) based on configuration
    /// </summary>
    public abstract class MainDb<TDb> where TDb: INamed
    {
        protected readonly Lazy<TDb> db;
        private readonly IConfiguration configuration;
        private readonly ILogger<MainDb<TDb>> log;

        protected MainDb(Lazy<IEnumerable<TDb>> dbs, IConfiguration configuration, ILogger<MainDb<TDb>> log)
        {
            this.configuration = configuration;
            this.log = log;

            db = new Lazy<TDb>(() => GetDb(dbs));
        }

        public string Name => "main";

        private TDb GetDb(Lazy<IEnumerable<TDb>> dbs)
        {
            var dbName = configuration.GetConnectionStringOrThrow("db");
            log.LogInformation($"use db '{dbName}' as {typeof(TDb)}");
            return dbs.Value.FirstOrDefault(db => db.Name == dbName && dbName != Name)
                ?? throw new PhysException($"db '{dbName}' not found");
        }
    }
}
