using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Phys.Lib.Db;
using Phys.Lib.Db.Files;
using Phys.Lib.Db.Migrations;

namespace Phys.Lib.Core.Files
{
    internal class FilesDbs : MainDb<IFilesDb>, IFilesDb
    {
        public FilesDbs(Lazy<IEnumerable<IFilesDb>> dbs, IConfiguration configuration, ILogger<FilesDbs> log)
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
