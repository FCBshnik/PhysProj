using Phys.Lib.Core.Migration;
using Phys.Lib.Db.Authors;
using Phys.Lib.Db.Migrations;
using Phys.Lib.Db.Search;
using Phys.Lib.Db.Works;
using Phys.Shared.Search;

namespace Phys.Lib.Core.Search
{
    internal class WorksTextSearchMigrator : BaseTextSearchMigrator<WorkDbo, WorkTso>
    {
        private readonly Lazy<IEnumerable<IAuthorsDb>> authorsDbs;

        public WorksTextSearchMigrator(string name,
            IEnumerable<IDbReader<WorkDbo>> readers, ITextSearch<WorkTso> textSearch, Lazy<IEnumerable<IAuthorsDb>> authorsDbs)
            : base(name, readers, textSearch)
        {
            this.authorsDbs = authorsDbs;
        }

        public override void Migrate(MigrationDto migration, IProgress<MigrationDto> progress)
        {
            IDbReaderResult<WorkDbo> result = null!;

            var source = readers.First(r => r.Name == migration.Source);
            var authorsDb = authorsDbs.Value.First(r => r.Name == migration.Source);
            textSearch.Reset();

            do
            {
                result = source.Read(new DbReaderQuery(100, result?.Cursor));
                textSearch.Add(result.Values.Where(Use).Select(w => Map(w, authorsDb)).ToList());
                migration.Stats.Updated += result.Values.Count;
                progress.Report(migration);
            } while (!result.IsCompleted);
        }

        protected override bool Use(WorkDbo value)
        {
            return value.Infos.Count > 0;
        }

        protected override WorkTso Map(WorkDbo value)
        {
            throw new NotImplementedException();
        }

        protected WorkTso Map(WorkDbo work, IAuthorsDb authorsDb)
        {
            var workTso = new WorkTso
            {
                Code = work.Code,
                Names = work.Infos.ToDictionary(i => i.Language, i => i.Name),
            };

            foreach (var author in authorsDb.Find(new AuthorsDbQuery { Codes = work.AuthorsCodes }))
            {
                foreach (var info in author.Infos)
                {
                    if (!workTso.Authors.TryGetValue(info.Language, out var names))
                        workTso.Authors.Add(info.Language, names = new List<string?>());
                    names.Add(info.FullName);
                }
            }

            return workTso;
        }
    }
}
