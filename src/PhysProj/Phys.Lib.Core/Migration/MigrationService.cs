using FluentValidation;
using Microsoft.Extensions.Logging;
using Phys.Shared;
using Phys.HistoryDb;
using Phys.Shared.Utils;
using Phys.Queue;

namespace Phys.Lib.Core.Migration
{
    internal class MigrationService : IMigrationService
    {
        private readonly List<IMigrator> migrators;
        private readonly IHistoryDb<MigrationDto> migrationsHistory;
        private readonly IMessageQueue queue;
        private readonly ILogger<MigrationService> log;

        public MigrationService(IEnumerable<IMigrator> migrators,
            IHistoryDb<MigrationDto> migrationsHistory,
            IMessageQueue queue,
            ILogger<MigrationService> log)
        {
            this.migrators = migrators.ToList();
            this.migrationsHistory = migrationsHistory;
            this.queue = queue;
            this.log = log;
        }

        public MigrationDto Create(MigrationParams task)
        {
            ArgumentNullException.ThrowIfNull(task);

            var migrator = migrators.Find(r => string.Equals(r.Name, task.Migrator, StringComparison.OrdinalIgnoreCase));
            if (migrator == null)
                throw new ValidationException($"migrator '{task.Migrator}' not found");
            if (task.Source == task.Destination)
                throw new ValidationException($"source and destination must differ");

            var migration = new MigrationDto
            {
                Id = migrationsHistory.GetNewId(),
                Migrator = task.Migrator,
                Source = task.Source,
                Destination = task.Destination,
                CreatedAt = DateTime.UtcNow,
                Status = "created",
            };
            migrationsHistory.Save(migration);
            log.LogInformation($"created {migration}");

            queue.Send(new MigrationMessage { Migration = migration });

            return migration;
        }

        public void Execute(MigrationDto migration)
        {
            ArgumentNullException.ThrowIfNull(migration);

            try
            {
                migration.StartedAt = DateTime.UtcNow;
                migration.Status = "migrating";
                migrationsHistory.Save(migration);
                log.LogInformation($"migration '{migration.Id}' started");

                var migrator = migrators.Find(r => string.Equals(r.Name, migration.Migrator, StringComparison.OrdinalIgnoreCase));
                if (migrator == null)
                    throw new PhysException($"migrator '{migration.Migrator}' not found");

                var progress = new ProgressWithInterval<MigrationDto>(migrationsHistory.Save, TimeSpan.FromSeconds(1));
                migrator.Migrate(migration, progress);

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

        public MigrationDto Get(string id)
        {
            ArgumentNullException.ThrowIfNull(id);

            return migrationsHistory.Get(id) ?? throw new PhysException($"migration '{id}' not found");
        }

        public List<MigrationDto> ListHistory(HistoryDbQuery query)
        {
            ArgumentNullException.ThrowIfNull(query);

            return migrationsHistory.List(query);
        }

        public List<IMigrator> ListMigrators()
        {
            return migrators;
        }
    }
}
