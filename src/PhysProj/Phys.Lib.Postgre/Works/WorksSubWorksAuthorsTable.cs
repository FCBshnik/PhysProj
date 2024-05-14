using Microsoft.Extensions.Logging;

namespace Phys.Lib.Postgres.Works
{
    internal class WorksSubWorksAuthorsTable : PostgresTable
    {
        public WorksSubWorksAuthorsTable(string tableName, ILogger<WorksSubWorksAuthorsTable> logger) : base(tableName, logger)
        {
        }
    }
}
