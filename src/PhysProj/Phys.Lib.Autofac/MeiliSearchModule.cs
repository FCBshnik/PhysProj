using Autofac;
using Meilisearch;
using Microsoft.Extensions.Logging;
using Phys.Lib.Core.Migration;
using Phys.Lib.Core.Search;
using Phys.Lib.Search;

namespace Phys.Lib.Autofac
{
    public class MeilisearchModule : Module
    {
        private readonly ILoggerFactory loggerFactory;
        private readonly ILogger log;
        private readonly string connectionString;
        private readonly string indexPrefix;

        public MeilisearchModule(string connectionString, string indexPrefix, ILoggerFactory loggerFactory)
        {
            ArgumentNullException.ThrowIfNull(connectionString);
            ArgumentNullException.ThrowIfNull(indexPrefix);
            ArgumentNullException.ThrowIfNull(loggerFactory);

            this.loggerFactory = loggerFactory;
            this.connectionString = connectionString;
            this.indexPrefix = indexPrefix;

            log = loggerFactory.CreateLogger<MeilisearchModule>();
        }

        protected override void Load(ContainerBuilder builder)
        {
            log.LogInformation($"meilisearch connection: {connectionString}");

            var client = new MeilisearchClient(connectionString, "123456");

            builder.RegisterType<WorksTextSearch>()
                .WithParameter(TypedParameter.From(client.Index($"{indexPrefix}-works")))
                .As<ITextSearch<WorkTso>>()
                .SingleInstance();

            builder.RegisterType<AuthorsTextSearch>()
                .WithParameter(TypedParameter.From(client.Index($"{indexPrefix}-authors")))
                .As<ITextSearch<AuthorTso>>()
                .SingleInstance();

            builder.RegisterType<WorksTextSearchMigrator>().WithParameter(TypedParameter.From(MigratorName.WorksSearch))
                .As<IMigrator>()
                .SingleInstance();
            builder.RegisterType<AuthorsTextSearchMigrator>().WithParameter(TypedParameter.From(MigratorName.AuthorsSearch))
                .As<IMigrator>()
                .SingleInstance();
        }
    }
}
