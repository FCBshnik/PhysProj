using Autofac;
using Microsoft.Extensions.Logging;
using Npgsql;
using Phys.Lib.Db.Authors;
using Phys.Lib.Db.Files;
using Phys.Lib.Db.Users;
using Phys.Lib.Db.Works;
using Phys.Lib.Postgres.Authors;
using Phys.Lib.Postgres.Files;
using Phys.Lib.Postgres.Users;
using Phys.Lib.Postgres.Works;

namespace Phys.Lib.Postgres
{
    public class PostgresModule : Module
    {
        private readonly ILoggerFactory loggerFactory;
        private readonly ILogger log;
        private readonly string connectionString;

        public PostgresModule(string connectionString, ILoggerFactory loggerFactory)
        {
            this.connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
            this.loggerFactory = loggerFactory;
            log = loggerFactory.CreateLogger<PostgresModule>();
        }

        protected override void Load(ContainerBuilder builder)
        {
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

        private void RegisterTable<Table, As>(ContainerBuilder builder, string tableName) where Table : PostgresTable where As : notnull
        {
            builder.RegisterType<Table>().WithParameter(TypedParameter.From(tableName))
                .As<As>().AsImplementedInterfaces().SingleInstance();
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
