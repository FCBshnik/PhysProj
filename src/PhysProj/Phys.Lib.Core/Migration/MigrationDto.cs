using Phys.HistoryDb;

namespace Phys.Lib.Core.Migration
{
    public class MigrationDto : IHistoryDbo
    {
        public string Id { get; set; }

        public required string Migrator { get; set; }

        public required string Source { get; set; }

        public required string Destination { get; set; }

        public required string Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? StartedAt { get; set; }

        public DateTime? CompletedAt { get; set; }

        public string? Result { get; set; }

        public string? Error { get; set; }

        public StatsDto Stats { get; set; } = new StatsDto();

        public class StatsDto
        {
            public int Created { get; set; }

            public int Updated { get; set; }

            public int Skipped { get; set; }

            public int Failed { get; set; }
        }
    }
}
