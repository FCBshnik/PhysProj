using Dapper;
using Microsoft.Extensions.Logging;
using Phys.Lib.Db.Migrations;
using SqlKata;
using SqlKata.Compilers;
using System.Data;

namespace Phys.Lib.Postgres
{
    internal class PostgresTable
    {
        protected static readonly PostgresCompiler compiler = new PostgresCompiler();

        protected readonly ILogger log;
        protected readonly string tableName;

        public string TableName => tableName;

        public PostgresTable(string tableName, ILogger log)
        {
            this.tableName = tableName;
            this.log = log;
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
            log.LogDebug($"exec: {sql.RawSql}");
            cnx.ExecuteScalar(sql.Sql, sql.NamedBindings);
        }

        internal void Delete(IDbConnection cnx, Func<Query, Query> where)
        {
            var query = new Query(TableName);
            where(query);
            var sql = compiler.Compile(query.AsDelete());
            log.LogDebug($"exec: {sql.RawSql}");
            cnx.ExecuteScalar(sql.Sql, sql.NamedBindings);
        }

        internal int Execute(IDbConnection cnx, Query update, Action<SqlResult>? hook = null)
        {
            var sql = compiler.Compile(update);
            hook?.Invoke(sql);
            log.LogDebug($"exec: {sql.RawSql}");
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

        protected IDbReaderResult<TDbo> Read<TDbo, TModel>(IDbConnection cnx, DbReaderQuery query, string idColumn, Func<TModel, TDbo> map)
            where TModel : IPostgresModel
        {
            ArgumentNullException.ThrowIfNull(query);

            var cmd = new Query(tableName).Limit(query.Limit).OrderBy(idColumn);
            if (query.Cursor != null)
                cmd = cmd.Where(idColumn, ">", int.Parse(query.Cursor));

            var users = Find<TModel>(cnx, cmd);
            return new DbReaderResult<TDbo>(users.Select(map).ToList(), users.LastOrDefault()?.Id.ToString());
        }
    }
}
