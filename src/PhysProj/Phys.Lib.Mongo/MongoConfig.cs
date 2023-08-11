using MongoDB.Bson.Serialization.Conventions;

namespace Phys.Lib.Mongo
{
    internal static class MongoConfig
    {
        public static void Configure()
        {
            ConfigureConventions();
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
