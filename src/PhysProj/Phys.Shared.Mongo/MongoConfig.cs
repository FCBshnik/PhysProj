using MongoDB.Bson.Serialization.Conventions;

namespace Phys.Shared.Mongo
{
    public static class MongoConfig
    {
        public static void ConfigureConventions()
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
