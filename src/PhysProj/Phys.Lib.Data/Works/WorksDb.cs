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

        public WorkDbo Update(string id, WorkUpdate work)
        {
            if (id is null) throw new ArgumentNullException(nameof(id));

            var filter = filterBuilder.Eq(i => i.Id, id);
            var update = updateBuilder.Combine();

            if (work.Date == string.Empty)
                update = update.Unset(i => i.Date);
            else if (work.Date != null)
                update = update.Set(i => i.Date, work.Date);

            if (work.Language == string.Empty)
                update = update.Unset(i => i.Language);
            else if (work.Language != null)
                update = update.Set(i => i.Language, work.Language);

            if (work.AddInfo != null)
                update = update.Push(i => i.Infos, work.AddInfo);
            if (work.DeleteInfo != null)
                update = update.PullFilter(i => i.Infos, i => i.Language == work.DeleteInfo);

            if (work.AddAuthor != null)
                update = update.AddToSet(i => i.AuthorsCodes, work.AddAuthor.Code);
            if (work.DeleteAuthor != null)
                update = update.Pull(i => i.AuthorsCodes, work.DeleteAuthor);

            if (work.AddWork != null)
                update = update.AddToSet(i => i.WorksCodes, work.AddWork.Code);
            if (work.DeleteWork != null)
                update = update.Pull(i => i.WorksCodes, work.DeleteWork);

            if (work.Original == WorkDbo.None)
                update = update.Unset(i => i.OriginalCode);
            else if (work.Original != null)
                update = update.Set(i => i.OriginalCode, work.Original.Code);

            return collection.FindOneAndUpdate(filter, update, findOneAndUpdateReturnAfter)
                ?? throw new ApplicationException($"work '{id}' was not updated due to not found in db");
        }
    }
}
