using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Phys.Lib.Db;
using Phys.Lib.Db.Files;
using Phys.Lib.Db.Migrations;

namespace Phys.Lib.Core.Files
{
    internal class MainFilesDb : MainDb<IFilesDb>, IFilesDb
    {
        public MainFilesDb(Lazy<IEnumerable<IFilesDb>> dbs, IConfiguration configuration, ILogger<MainFilesDb> log)
            :base(dbs, configuration, log)
        {
        }

        public List<FileDbo> Find(FilesDbQuery query)
        {
            return db.Value.Find(query);
        }

        public void Create(FileDbo file)
        {
            db.Value.Create(file);
        }

        public void Update(string code, FileDbUpdate update)
        {
            db.Value.Update(code, update);
        }

        public void Delete(string code)
        {
            db.Value.Delete(code);
        }

        public IDbReaderResult<FileDbo> Read(DbReaderQuery query)
        {
            return db.Value.Read(query);
        }
    }
}
