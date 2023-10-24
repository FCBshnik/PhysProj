using Phys.Lib.Core.Migration;
using Phys.Lib.Db.Authors;

namespace Phys.Lib.Core.Authors
{
    internal class AuthorsWriter : IMigrationWriter<AuthorDbo>
    {
        private readonly IAuthorsDb db;

        public AuthorsWriter(IAuthorsDb db)
        {
            this.db = db;
        }

        public string Name => db.Name;

        public void Write(IEnumerable<AuthorDbo> values, MigrationDto.StatsDto stats)
        {
            foreach (var author in values)
            {
                var prev = db.FindByCode(author.Code);
                if (author.Equals(prev))
                {
                    stats.Skipped++;
                    continue;
                }

                if (prev != null)
                    foreach (var info in prev.Infos)
                        db.Update(author.Code, new AuthorDbUpdate { DeleteInfo = info.Language });
                else
                    db.Create(author.Code);

                db.Update(author.Code, new AuthorDbUpdate { Born = author.Born, Died = author.Died, });
                foreach (var info in author.Infos)
                    db.Update(author.Code, new AuthorDbUpdate { AddInfo = info });

                _ = prev == null ? stats.Created++ : stats.Updated++;
            }
        }
    }
}
