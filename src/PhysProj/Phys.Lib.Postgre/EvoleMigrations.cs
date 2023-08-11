using EvolveDb;
using NLog;
using System.Data.Common;

namespace Phys.Lib.Postgres
{
    internal static class EvoleMigrations
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        public static void Migrate(DbConnection cnx)
        {
            var evolve = new Evolve(cnx, l => log.Info($"{l}"))
            {
                Locations = new[] { "db/postgres/migrations" },
                IsEraseDisabled = true,
            };
            evolve.Migrate();
        }
    }
}
