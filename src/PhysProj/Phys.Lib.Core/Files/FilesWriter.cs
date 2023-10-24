using Phys.Lib.Core.Migration;
using Phys.Lib.Db.Files;

namespace Phys.Lib.Core.Files
{
    internal class FilesWriter : IMigrationWriter<FileDbo>
    {
        private readonly IFilesDb db;

        public FilesWriter(IFilesDb db)
        {
            this.db = db;
        }

        public string Name => db.Name;

        public void Write(IEnumerable<FileDbo> values, MigrationDto.StatsDto stats)
        {
            foreach (var file in values)
            {
                var prev = db.Find(new FilesDbQuery { Code = file.Code }).FirstOrDefault();
                if (file.Equals(prev))
                {
                    stats.Skipped++;
                    continue;
                }

                if (prev != null)
                    prev.Links.ForEach(l => db.Update(file.Code, new FileDbUpdate { DeleteLink = l }));
                else
                    db.Create(file);

                file.Links.ForEach(l => db.Update(file.Code, new FileDbUpdate { AddLink = l }));

                _ = prev == null ? stats.Created++ : stats.Updated++;
            }
        }
    }
}
