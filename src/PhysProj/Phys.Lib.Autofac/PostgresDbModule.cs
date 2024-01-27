using Autofac;
using Microsoft.Extensions.Logging;
using Npgsql;
using Phys.Lib.Db.Authors;
using Phys.Lib.Db.Files;
using Phys.Lib.Db.Users;
using Phys.Lib.Db.Works;
using Phys.Lib.Postgres;
using Phys.Lib.Postgres.Authors;
using Phys.Lib.Postgres.Files;
using Phys.Lib.Postgres.Users;
using Phys.Lib.Postgres.Works;

namespace Phys.Lib.Autofac
{
    public class PostgresDbModule : Module
    {
        private const string dbTypeName = "postgres";

        private readonly ILoggerFactory loggerFactory;
        private readonly ILogger log;
        private readonly string connectionString;

        public PostgresDbModule(string connectionString, ILoggerFactory loggerFactory)
        {
            ArgumentNullException.ThrowIfNull(connectionString);
            ArgumentNullException.ThrowIfNull(loggerFactory);

            this.connectionString = connectionString;
            this.loggerFactory = loggerFactory;

            log = loggerFactory.CreateLogger<PostgresDbModule>();
        }

        protected override void Load(ContainerBuilder builder)
        {
            log.LogInformation($"postgres server: {connectionString}");

            DapperConfig.Configure();

            builder.Register(_ => CreateNpgsqlDataSource())
                .SingleInstance()
                .AsSelf();

            RegisterTable<UsersDb, IUsersDb>(builder, "users");
            RegisterTable<AuthorsDb, IAuthorsDb>(builder, "authors");
            RegisterTable<WorksDb, IWorksDb>(builder, "works");
            RegisterTable<FilesDb, IFilesDb>(builder, "files");
            RegisterTable<FilesLinksTable, FilesLinksTable>(builder, "files_links");
            RegisterTable<AuthorsInfosTable, AuthorsInfosTable>(builder, "authors_infos");
            RegisterTable<WorksAuthorsTable, WorksAuthorsTable>(builder, "works_authors");
            RegisterTable<WorksInfosTable, WorksInfosTable>(builder, "works_infos");
            RegisterTable<WorksSubWorksTable, WorksSubWorksTable>(builder, "works_sub_works");
            RegisterTable<WorksFilesTable, WorksFilesTable>(builder, "works_files");
        }

        private void RegisterTable<ImplDb, IDb>(ContainerBuilder builder, string tableName) where ImplDb : PostgresTable where IDb : notnull
        {
            builder.RegisterType<ImplDb>().WithParameter(TypedParameter.From(tableName))
                .As<IDb>().Named<IDb>(dbTypeName).AsImplementedInterfaces()
                .SingleInstance();
        }

        private NpgsqlDataSource CreateNpgsqlDataSource()
        {
            log.LogInformation($"postgres connection: {connectionString}");

            var dataSource = new NpgsqlDataSourceBuilder(connectionString)
                .EnableParameterLogging(false)
                .UseLoggerFactory(loggerFactory)
                .Build();

            // don't resolve until migrations completed
            using (var cnx = dataSource.OpenConnection())
                EvoleMigrations.Migrate(cnx, loggerFactory.CreateLogger(typeof(EvoleMigrations)));

            return dataSource;
        }
    }
}
