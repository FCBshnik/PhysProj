using EvolveDb;
using Microsoft.Extensions.Logging;
using System.Data.Common;

namespace Phys.Lib.Postgres
{
    internal static class EvoleMigrations
    {
        public static void Migrate(DbConnection cnx, ILogger logger)
        {
            var migrationsDirectory = new DirectoryInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "db/postgres/migrations"));

            logger.LogInformation($"migrations files at '{migrationsDirectory.FullName}': {migrationsDirectory.EnumerateFiles("*.sql").Count()} files");

            var evolve = new Evolve(cnx, l => logger.LogInformation($"{l}"))
            {
                Locations = new[] { migrationsDirectory.FullName },
                IsEraseDisabled = true,
            };

            evolve.Migrate();
        }
    }
}
