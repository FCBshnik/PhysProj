using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using Phys.Lib.Core.Authors;
using Phys.Lib.Core.Users;

namespace Phys.Lib.Data
{
    internal static class MongoConfig
    {
        private static readonly IBsonSerializer idSerializer = new StringSerializer(BsonType.ObjectId);

        public static void Configure()
        {
            ConfigureConventions();

            BsonClassMap.RegisterClassMap<UserDbo>(m =>
            {
                m.AutoMap();
                m.MapIdProperty(u => u.Id).SetSerializer(idSerializer);
                m.MapProperty(u => u.NameLowerCase).SetElementName("nameLc");
                m.MapProperty(u => u.PasswordHash).SetElementName("pwdHash");
            });

            BsonClassMap.RegisterClassMap<AuthorDbo>(m =>
            {
                m.AutoMap();
                m.MapIdProperty(u => u.Id).SetSerializer(idSerializer);
            });

            BsonClassMap.RegisterClassMap<AuthorDbo.InfoDbo>(m =>
            {
                m.AutoMap();
                m.MapProperty(u => u.Language).SetElementName("lang");
                m.MapProperty(u => u.FullName).SetElementName("name");
                m.MapProperty(u => u.Description).SetElementName("desc");
            });
        }

        private static void ConfigureConventions()
        {
            var conventions = new ConventionPack
            {
                new CamelCaseElementNameConvention(),
                new IgnoreIfNullConvention(true),
                new IgnoreExtraElementsConvention(true)
            };

            ConventionRegistry.Register("app", conventions, _ => true);
        }
    }
}
