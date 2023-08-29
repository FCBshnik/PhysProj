using Microsoft.Extensions.Logging;

namespace Phys.Lib.Postgres.Files
{
    internal class FilesLinksTable : PostgresTable
    {
        public FilesLinksTable(string tableName, ILogger<FilesLinksTable> logger) : base(tableName, logger)
        {
        }
    }
}
