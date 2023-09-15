﻿using Microsoft.Extensions.Logging;
using Phys.Shared;
using Phys.Shared.HistoryDb;

namespace Phys.Lib.Core.Migration
{
    internal class MigrationService : IMigrationService
    {
        private readonly List<IMigrator> migrators;
        private readonly IHistoryDb<MigrationDto> migrationsHistory;
        private readonly ILogger<MigrationService> log;

        public MigrationService(IEnumerable<IMigrator> migrators,
            IHistoryDb<MigrationDto> migrationsHistory,
            ILogger<MigrationService> log)
        {
            this.migrators = migrators.ToList();
            this.migrationsHistory = migrationsHistory;
            this.log = log;
        }

        public MigrationDto Create(MigrationTask task)
        {
            ArgumentNullException.ThrowIfNull(task);

            var migrator = migrators.Find(r => string.Equals(r.Name, task.Migrator, StringComparison.OrdinalIgnoreCase));
            if (migrator == null)
                throw new PhysException($"migrator '{task.Migrator}' not found");

            var migration = new MigrationDto
            {
                Migrator = task.Migrator,
                Source = task.Source,
                Destination = task.Destination,
                Status = "created",
            };
            migrationsHistory.Save(migration);

            return migration;
        }

        public void Execute(MigrationDto migration)
        {
            ArgumentNullException.ThrowIfNull(migration);

            log.LogInformation($"migration '{migration.Id}' started");

            try
            {
                var migrator = migrators.Find(r => string.Equals(r.Name, migration.Migrator, StringComparison.OrdinalIgnoreCase));
                if (migrator == null)
                    throw new PhysException($"migrator '{migration.Migrator}' not found");

                migrator.Migrate(migration);
                migration.CompletedAt = DateTime.UtcNow;
                migration.Status = "completed";
                migration.Result = "success";
                migrationsHistory.Save(migration);
                log.LogInformation($"migration '{migration.Id}' completed");
            }
            catch (Exception e)
            {
                migration.CompletedAt = DateTime.UtcNow;
                migration.Status = "completed";
                migration.Result = "error";
                migration.Error ??= e.Message;
                migrationsHistory.Save(migration);
                log.LogError(e, $"migration '{migration.Id}' failed");
            }
        }
    }
}
