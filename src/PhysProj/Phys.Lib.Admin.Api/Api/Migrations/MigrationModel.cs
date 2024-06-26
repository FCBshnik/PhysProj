﻿using Phys.Lib.Core.Migration;

namespace Phys.Lib.Admin.Api.Api.Migrations
{
    public class MigrationModel
    {
        public required string Id { get; set; }

        public required string Migrator { get; set; }

        public required string Source { get; set; }

        public required string Destination { get; set; }

        public required string Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? StartedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public string? Result { get; set; }

        public string? Error { get; set; }

        public required StatsModel Stats { get; set; }

        public class StatsModel
        {
            public int Created { get; set; }

            public int Updated { get; set; }

            public int Skipped { get; set; }

            public int Failed { get; set; }

            public static StatsModel Map(MigrationDto.StatsDto migration)
            {
                return new StatsModel
                {
                    Created = migration.Created,
                    Updated = migration.Updated,
                    Skipped = migration.Skipped,
                    Failed = migration.Failed,
                };
            }
        }

        public static MigrationModel Map(MigrationDto migration)
        {
            return new MigrationModel
            {
                Id = migration.Id,
                Source = migration.Source,
                Destination = migration.Destination,
                Migrator = migration.Migrator,
                Status = migration.Status,
                Result = migration.Result,
                CreatedAt = migration.CreatedAt,
                StartedAt = migration.StartedAt,
                CompletedAt = migration.CompletedAt,
                Error = migration.Error,
                Stats = StatsModel.Map(migration.Stats),
            };
        }
    }
}
