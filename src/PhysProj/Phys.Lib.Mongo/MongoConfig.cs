using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using Phys.Lib.Db.Authors;
using Phys.Lib.Db.Files;
using Phys.Lib.Db.Users;
using Phys.Lib.Db.Works;

namespace Phys.Lib.Mongo
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

            BsonClassMap.RegisterClassMap<WorkDbo>(m =>
            {
                m.AutoMap();
                m.MapIdProperty(u => u.Id).SetSerializer(idSerializer);
                m.MapProperty(u => u.Language).SetElementName("lang");
                m.MapProperty(u => u.Publish).SetElementName("pubd");
                m.MapProperty(u => u.AuthorsCodes).SetElementName("auth");
                m.MapProperty(u => u.SubWorksCodes).SetElementName("subw");
                m.MapProperty(u => u.OriginalCode).SetElementName("orig");
                m.MapProperty(u => u.FilesCodes).SetElementName("files");
            });

            BsonClassMap.RegisterClassMap<WorkDbo.InfoDbo>(m =>
            {
                m.AutoMap();
                m.MapProperty(u => u.Language).SetElementName("lang");
                m.MapProperty(u => u.Name).SetElementName("name");
                m.MapProperty(u => u.Description).SetElementName("desc");
            });

            BsonClassMap.RegisterClassMap<FileDbo>(m =>
            {
                m.AutoMap();
                m.MapIdProperty(u => u.Id).SetSerializer(idSerializer);
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
