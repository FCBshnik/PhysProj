using Microsoft.Extensions.Logging;

namespace Phys.Lib.Postgres.Works
{
    internal class WorksInfosTable : PostgresTable
    {
        public WorksInfosTable(string tableName, ILogger<WorksInfosTable> logger) : base(tableName, logger)
        {
        }
    }
}
