using Autofac;
using MongoDB.Driver;
using NLog;
using Phys.Lib.Core.Users;
using Phys.Lib.Data.Users;

namespace Phys.Lib.Data
{
    public class DbModule : Module
    {
        private static readonly ILogger log = LogManager.GetCurrentClassLogger();

        private readonly string connectionString;

        public DbModule(string connectionString)
        {
            this.connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
        }

        protected override void Load(ContainerBuilder builder)
        {
            MongoConfig.Configure();

            log.Info($"mongo connection: {connectionString}");
            var client = new MongoClient(connectionString);

            builder.Register(c => client.GetDatabase("phys-lib"))
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.Register(c => c.Resolve<IMongoDatabase>().GetCollection<UserDbo>("users"))
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.RegisterTypes(new[] { typeof(UsersDb) } )
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}
