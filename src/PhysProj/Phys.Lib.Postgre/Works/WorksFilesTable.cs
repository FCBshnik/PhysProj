using Microsoft.Extensions.Logging;

namespace Phys.Lib.Postgres.Works
{
    internal class WorksFilesTable : PostgresTable
    {
        public WorksFilesTable(string tableName, ILogger<WorksFilesTable> logger) : base(tableName, logger)
        {
        }
    }
}
