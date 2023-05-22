using MongoDB.Driver;

namespace Phys.Lib.Data
{
    internal class Collection<T>
    {
        protected FilterDefinitionBuilder<T> Filter => Builders<T>.Filter;
    }
}
