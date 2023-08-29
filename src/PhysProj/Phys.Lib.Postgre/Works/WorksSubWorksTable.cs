using Microsoft.Extensions.Logging;

namespace Phys.Lib.Postgres.Works
{
    internal class WorksSubWorksTable : PostgresTable
    {
        public WorksSubWorksTable(string tableName, ILogger<WorksSubWorksTable> logger) : base(tableName, logger)
        {
        }
    }
}
