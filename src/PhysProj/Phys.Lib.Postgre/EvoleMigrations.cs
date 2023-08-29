using EvolveDb;
using Microsoft.Extensions.Logging;
using System.Data.Common;

namespace Phys.Lib.Postgres
{
    internal static class EvoleMigrations
    {
        public static void Migrate(DbConnection cnx, ILogger logger)
        {
            var evolve = new Evolve(cnx, l => logger.LogInformation($"{l}"))
            {
                Locations = new[] { "db/postgres/migrations" },
                IsEraseDisabled = true,
            };
            evolve.Migrate();
        }
    }
}
