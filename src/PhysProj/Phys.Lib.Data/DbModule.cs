using Autofac;
using MongoDB.Driver;
using NLog;
using Phys.Lib.Core.Authors;
using Phys.Lib.Core.Files;
using Phys.Lib.Core.Users;
using Phys.Lib.Core.Works;
using Phys.Lib.Data.Authors;
using Phys.Lib.Data.Files;
using Phys.Lib.Data.Users;
using Phys.Lib.Data.Works;

namespace Phys.Lib.Data
{
    public class DbModule : Module
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly string connectionString;

        public DbModule(string connectionString)
        {
            this.connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        protected override void Load(ContainerBuilder builder)
        {
            MongoConfig.Configure();

            log.Info($"mongo connection: {connectionString}");
            var client = new MongoClient(new MongoUrl(connectionString));

            builder.Register(_ => client.GetDatabase("phys-lib"))
                .AsImplementedInterfaces()
                .SingleInstance();

            RegisterCollection<UserDbo, UsersDb>(builder, "users");
            RegisterCollection<AuthorDbo, AuthorsDb>(builder, "authors");
            RegisterCollection<WorkDbo, WorksDb>(builder, "works");
            RegisterCollection<FileDbo, FilesDb>(builder, "files");
        }

        private void RegisterCollection<TItem, TDb>(ContainerBuilder builder, string collectionName) where TDb: Collection<TItem>
        {
            builder.Register(c => c.Resolve<IMongoDatabase>().GetCollection<TItem>(collectionName))
                .AsImplementedInterfaces().SingleInstance();

            builder.RegisterType<TDb>().AsImplementedInterfaces().SingleInstance();
        }
    }
}
