using Microsoft.Extensions.Logging;

namespace Phys.Lib.Postgres.Works
{
    internal class WorksAuthorsTable : PostgresTable
    {
        public WorksAuthorsTable(string tableName, ILogger<WorksAuthorsTable> logger) : base(tableName, logger)
        {
        }
    }
}
