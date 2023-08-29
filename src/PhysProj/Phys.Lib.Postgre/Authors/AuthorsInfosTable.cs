using Microsoft.Extensions.Logging;

namespace Phys.Lib.Postgres.Authors
{
    internal class AuthorsInfosTable : PostgresTable
    {
        public AuthorsInfosTable(string tableName, ILogger<AuthorsInfosTable> logger) : base(tableName, logger)
        {
        }
    }
}
