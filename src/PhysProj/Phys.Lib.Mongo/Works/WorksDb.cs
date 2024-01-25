using MongoDB.Bson;
using MongoDB.Driver;
using System.Text.RegularExpressions;
using Phys.Lib.Mongo.Utils;
using Phys.Lib.Db.Works;
using Microsoft.Extensions.Logging;
using Phys.Lib.Db;
using Phys.Lib.Db.Migrations;

namespace Phys.Lib.Mongo.Works
{
    internal class WorksDb : Collection<WorkModel>, IWorksDb
    {
        public string Name { get; }

        public WorksDb(Lazy<IMongoCollection<WorkModel>> collection, string name, ILogger<WorksDb> logger) : base(collection, logger)
        {
            Name = name;
        }

        protected override void Init(IMongoCollection<WorkModel> collection)
        {
            collection.Indexes.CreateOne(new CreateIndexModel<WorkModel>(IndexBuilder.Ascending(i => i.Code), new CreateIndexOptions { Unique = true }));
        }

        public void Create(string code)
        {
            ArgumentNullException.ThrowIfNull(code);

            var work = new WorkModel
            {
                Code = code,
                Id = ObjectId.GenerateNewId().ToString()
            };

            Insert(work);
        }

        public void Delete(string code)
        {
            ArgumentNullException.ThrowIfNull(code);

            MongoCollection.DeleteOne(FilterBuilder.Eq(i => i.Code, code));
        }

        public List<WorkDbo> Find(WorksDbQuery query)
        {
            ArgumentNullException.ThrowIfNull(query);

            var filter = FilterBuilder.Empty;
            if (query.Code != null)
                filter = FilterBuilder.And(filter, FilterBuilder.Eq(u => u.Code, query.Code));
            if (query.Codes != null)
                filter = FilterBuilder.And(filter, FilterBuilder.In(u => u.Code, query.Codes));
            if (query.AuthorCode != null)
                filter = FilterBuilder.And(filter, FilterBuilder.AnyEq(u => u.AuthorsCodes, query.AuthorCode));
            if (query.FileCode != null)
                filter = FilterBuilder.And(filter, FilterBuilder.AnyEq(u => u.FilesCodes, query.FileCode));
            if (query.OriginalCode != null)
                filter = FilterBuilder.And(filter, FilterBuilder.Eq(u => u.OriginalCode, query.OriginalCode));
            if (query.SubWorkCode != null)
                filter = FilterBuilder.And(filter, FilterBuilder.AnyEq(u => u.SubWorksCodes, query.SubWorkCode));
            if (query.Search != null)
            {
                var regex = Regex.Escape(query.Search);
                var infoFilterBuilder = Builders<WorkModel.InfoModel>.Filter;
                filter = FilterBuilder.And(filter, FilterBuilder.Or(
                    FilterBuilder.Regex(u => u.Code, regex),
                    FilterBuilder.ElemMatch(u => u.Infos, infoFilterBuilder.Regex(i => i.Name, regex)),
                    FilterBuilder.ElemMatch(u => u.Infos, infoFilterBuilder.Regex(i => i.Description, regex))));
            }

            var sort = SortBuilder.Descending(i => i.Id);

            return MongoCollection.Find(filter).Limit(query.Limit).Sort(sort).ToList().Select(WorkMapper.Map).ToList();
        }

        public void Update(string code, WorkDbUpdate work)
        {
            ArgumentNullException.ThrowIfNull(code);
            ArgumentNullException.ThrowIfNull(work);

            var filter = FilterBuilder.Eq(i => i.Code, code);
            var update = UpdateBuilder.Set(i => i.UpdatedAt, DateTime.UtcNow);

            if (work.Publish.IsEmpty())
                update = update.Unset(i => i.Publish);
            else if (work.Publish.HasValue())
                update = update.Set(i => i.Publish, work.Publish);

            if (work.Language.IsEmpty())
                update = update.Unset(i => i.Language);
            else if (work.Language.HasValue())
                update = update.Set(i => i.Language, work.Language);

            if (work.IsPublic != null)
                update = update.Set(i => i.IsPublic, work.IsPublic.Value);

            if (work.AddInfo != null)
                update = update.Push(i => i.Infos, WorkMapper.Map(work.AddInfo));
            if (work.DeleteInfo.HasValue())
                update = update.PullFilter(i => i.Infos, i => i.Language == work.DeleteInfo);

            if (work.AddAuthor.HasValue())
                update = update.AddToSet(i => i.AuthorsCodes, work.AddAuthor);
            if (work.DeleteAuthor.HasValue())
                update = update.Pull(i => i.AuthorsCodes, work.DeleteAuthor);

            if (work.AddSubWork.HasValue())
                update = update.AddToSet(i => i.SubWorksCodes, work.AddSubWork);
            if (work.DeleteSubWork.HasValue())
                update = update.Pull(i => i.SubWorksCodes, work.DeleteSubWork);

            if (work.Original.IsEmpty())
                update = update.Unset(i => i.OriginalCode);
            else if (work.Original.HasValue())
                update = update.Set(i => i.OriginalCode, work.Original);

            if (work.AddFile.HasValue())
                update = update.AddToSet(i => i.FilesCodes, work.AddFile);
            if (work.DeleteFile.HasValue())
                update = update.Pull(i => i.FilesCodes, work.DeleteFile);

            if (MongoCollection.UpdateOne(filter, update).MatchedCount == 0)
                throw new PhysDbException($"work '{code}' update failed");
        }

        IDbReaderResult<WorkDbo> IDbReader<WorkDbo>.Read(DbReaderQuery query)
        {
            return Read(query, WorkMapper.Map);
        }
    }
}
