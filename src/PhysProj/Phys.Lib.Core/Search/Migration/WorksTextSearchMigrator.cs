using Phys.Lib.Core.Migration;
using Phys.Lib.Db.Authors;
using Phys.Lib.Db.Works;
using Phys.Lib.Search;

namespace Phys.Lib.Core.Search.Migration
{
    internal class WorksTextSearchMigrator : IMigrator
    {
        private readonly IEnumerable<IWorksDb> worksDbs;
        private readonly ITextSearch<WorkTso> textSearch;
        private readonly Lazy<IEnumerable<IAuthorsDb>> authorsDbs;

        public WorksTextSearchMigrator(string name, IEnumerable<IWorksDb> worksDbs, ITextSearch<WorkTso> textSearch, Lazy<IEnumerable<IAuthorsDb>> authorsDbs)
        {
            Name = name;
            this.worksDbs = worksDbs;
            this.textSearch = textSearch;
            this.authorsDbs = authorsDbs;
        }

        public string Name { get; }

        public IEnumerable<string> Sources => worksDbs.Select(r => r.Name);

        public IEnumerable<string> Destinations => new[] { "search" };

        public void Migrate(MigrationDto migration, IProgress<MigrationDto> progress)
        {
            MigrateAsync(migration, progress).Wait();
        }

        private async Task MigrateAsync(MigrationDto migration, IProgress<MigrationDto> progress)
        {
            var workDb = worksDbs.First(r => r.Name == migration.Source);
            var authorsDb = authorsDbs.Value.First(r => r.Name == migration.Source);
            await textSearch.Reset(Language.AllAsStrings);

            foreach (var works in workDb.Read(100))
            {
                var values = works.Where(IsSearchable).Select(w => Map(w, workDb, authorsDb)).ToList();
                await textSearch.Index(values);
                migration.Stats.Updated += values.Count;
                progress.Report(migration);
            }
        }

        private bool IsSearchable(WorkDbo work)
        {
            return work.IsPublic && work.Infos.Count > 0;
        }

        private WorkTso Map(WorkDbo work, IWorksDb worksDb, IAuthorsDb authorsDb)
        {
            var workTso = new WorkTso
            {
                Code = work.Code,
                Info = new WorkInfoTso { Code = work.Code },
                Names = work.Infos.ToDictionary(i => i.Language, i => i.Name),
            };

            if (work.AuthorsCodes.Count > 0)
            {
                foreach (var author in authorsDb.Find(new AuthorsDbQuery { Codes = work.AuthorsCodes }))
                {
                    foreach (var info in author.Infos)
                    {
                        if (!workTso.Authors.TryGetValue(info.Language, out var names))
                            workTso.Authors.Add(info.Language, names = new List<string?>());
                        names.Add(info.FullName);
                    }
                }
            }

            PopulateInfo(workTso.Info, work, new Dictionary<string, WorkInfoTso>(), worksDb);

            return workTso;
        }

        private void PopulateInfo(WorkInfoTso info, WorkDbo work, Dictionary<string, WorkInfoTso> visited, IWorksDb db)
        {
            if (visited.ContainsKey(work.Code))
                return;

            info.HasFiles = work.FilesCodes.Count > 0;
            info.IsPublic = work.IsPublic;
            visited[work.Code] = info;

            foreach (var subWorkCode in work.SubWorksCodes)
            {
                var subWorkInfo = new WorkInfoTso { Code = subWorkCode };
                info.SubWorks.Add(subWorkInfo);
                PopulateInfo(subWorkInfo, db.GetByCode(subWorkCode), visited, db);
            }

            foreach (var collected in db.Find(new WorksDbQuery { SubWorkCode = work.Code }))
            {
                var collectedInfo = new WorkInfoTso { Code = collected.Code };
                info.Collected.Add(collectedInfo);
                PopulateInfo(collectedInfo, db.GetByCode(collected.Code), visited, db);
            }
        }
    }
}
