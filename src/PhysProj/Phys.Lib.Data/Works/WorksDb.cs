using MongoDB.Bson;
using MongoDB.Driver;
using Phys.Lib.Core.Works;

namespace Phys.Lib.Data.Works
{
    internal class WorksDb : Collection<WorkDbo>, IWorksDb
    {
        public WorksDb(IMongoCollection<WorkDbo> collection) : base(collection)
        {
        }

        public WorkDbo Create(WorkDbo work)
        {
            work.Id = ObjectId.GenerateNewId().ToString();
            return Insert(work);
        }

        public void Delete(string id)
        {
            if (id is null) throw new ArgumentNullException(nameof(id));

            collection.DeleteOne(filterBuilder.Eq(i => i.Id, id));
        }

        public List<WorkDbo> Find(WorksQuery query)
        {
            var filter = filterBuilder.Empty;
            if (query.Code != null)
                filter = filterBuilder.And(filter, filterBuilder.Eq(u => u.Code, query.Code));
            if (query.Search != null)
                filter = filterBuilder.And(filter, filterBuilder.Regex(u => u.Code, query.Search));

            var sort = sortBuilder.Descending(i => i.Id);

            return collection.Find(filter).Limit(query.Limit).Sort(sort).ToList();
        }

        public WorkDbo Get(string id)
        {
            if (id is null) throw new ArgumentNullException(nameof(id));

            return collection.Find(filterBuilder.Eq(u => u.Id, id)).FirstOrDefault() ?? throw new ApplicationException($"work '{id}' not found in db");
        }

        public WorkDbo Update(string id, WorkUpdate options)
        {
            if (id is null) throw new ArgumentNullException(nameof(id));

            var filter = filterBuilder.Eq(i => i.Id, id);

            var update = updateBuilder.Combine();
            if (options.Date != null)
                update = update.Set(i => i.Date, options.Date);
            if (options.Language != null)
                update = update.Set(i => i.Language, options.Language);
            if (options.AddInfo != null)
                update = update.Push(i => i.Infos, options.AddInfo);
            if (options.RemoveInfo != null)
                update = update.PullFilter(i => i.Infos, i => i.Language == options.RemoveInfo);
            if (options.AddAuthor != null)
                update = update.Push(i => i.AuthorsIds, options.AddAuthor);
            if (options.RemoveAuthor != null)
                update = update.Pull(i => i.AuthorsIds, options.RemoveAuthor);
            if (options.AddEdition != null)
                update = update.Push(i => i.EditionsIds, options.AddEdition);
            if (options.RemoveEdition != null)
                update = update.Pull(i => i.EditionsIds, options.RemoveEdition);

            return collection.FindOneAndUpdate(filter, update, findOneAndUpdateReturnAfter)
                ?? throw new ApplicationException($"work '{id}' was not updated due to not found in db");
        }
    }
}
