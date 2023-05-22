using Autofac;
using MongoDB.Driver;
using Phys.Lib.Core.Users;
using Phys.Lib.Data.Users;

namespace Phys.Lib.Data
{
    public class DbModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            DbConfig.Configure();

            var client = new MongoClient("mongodb://root:123456@192.168.2.46:27017");

            builder.Register(c => client.GetDatabase("phys-lib"))
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.Register(c => c.Resolve<IMongoDatabase>().GetCollection<UserDbo>("users"))
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.RegisterType<Db>()
                .AsImplementedInterfaces()
                .SingleInstance();

            builder.RegisterTypes(new[] { typeof(UsersDb) } )
                .AsImplementedInterfaces()
                .SingleInstance();
        }
    }
}
