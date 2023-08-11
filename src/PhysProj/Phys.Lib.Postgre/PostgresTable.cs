using Dapper;
using NLog;
using SqlKata;
using SqlKata.Compilers;
using System.Data;

namespace Phys.Lib.Postgres
{
    internal class PostgresTable
    {
        protected static readonly PostgresCompiler compiler = new PostgresCompiler();

        protected readonly Logger log;
        protected readonly string tableName;

        public string TableName => tableName;

        public PostgresTable(string tableName)
        {
            this.tableName = tableName;
            log = LogManager.GetLogger($"db-pg_{tableName}");
        }

        internal void Insert(IDbConnection cnx, object insertObj)
        {
            var command = new Query(tableName).AsInsert(insertObj, returnId: false);
            var sql = compiler.Compile(command);
            cnx.ExecuteScalar(sql.Sql, sql.NamedBindings);
        }

        internal void Delete(IDbConnection cnx, Query delete)
        {
            var sql = compiler.Compile(delete.AsDelete());
            log.Info($"exec: {sql.RawSql}");
            cnx.ExecuteScalar(sql.Sql, sql.NamedBindings);
        }

        internal void Delete(IDbConnection cnx, Func<Query, Query> where)
        {
            var query = new Query(TableName);
            where(query);
            var sql = compiler.Compile(query.AsDelete());
            log.Info($"exec: {sql.RawSql}");
            cnx.ExecuteScalar(sql.Sql, sql.NamedBindings);
        }

        internal int Execute(IDbConnection cnx, Query update, Action<SqlResult>? hook = null)
        {
            var sql = compiler.Compile(update);
            hook?.Invoke(sql);
            log.Info($"exec: {sql.RawSql}");
            return cnx.Execute(sql.Sql, sql.NamedBindings);
        }

        internal List<T> Find<T>(IDbConnection cnx, Query query)
        {
            var sql = compiler.Compile(query);
            return cnx.Query<T>(sql.Sql, sql.NamedBindings).ToList();
        }

        internal List<T> FindJoin<T, S>(IDbConnection cnx, Query query, string on, Func<T, S, T> map)
        {
            var sql = compiler.Compile(query);
            return cnx.Query(sql.Sql, map, sql.NamedBindings, splitOn: on).ToList();
        }
    }
}
