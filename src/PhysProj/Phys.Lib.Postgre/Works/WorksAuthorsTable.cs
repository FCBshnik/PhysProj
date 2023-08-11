using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phys.Lib.Postgres.Works
{
    internal class WorksAuthorsTable : PostgresTable
    {
        public WorksAuthorsTable(string tableName) : base(tableName)
        {
        }
    }
}
