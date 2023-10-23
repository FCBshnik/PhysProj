using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Phys.Lib.Db.Files;
using Phys.Lib.Db.Migrations;
using Phys.Shared;

namespace Phys.Lib.Core.Files
{
    internal class FilesDbs : IFilesDbs
    {
        private readonly IEnumerable<IDbReader<FileDbo>> readers;
        private readonly IConfiguration configuration;
        private readonly ILogger<FilesDbs> log;
        private readonly Lazy<IFilesDb> db;

        public string Name => "main";

        public FilesDbs(IEnumerable<IFilesDb> dbs, IEnumerable<IDbReader<FileDbo>> readers, IConfiguration configuration, ILogger<FilesDbs> log)
        {
            this.readers = readers;
            this.configuration = configuration;
            this.log = log;

            db = new Lazy<IFilesDb>(() => GetDb(dbs));
        }

        public IDbReader<FileDbo> GetReader()
        {
            var reader = readers.FirstOrDefault(r => r.Name == db.Value.Name);
            if (reader == null)
                throw new PhysException($"there is no file db reader '{db.Value.Name}'");

            return reader;
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

        private IFilesDb GetDb(IEnumerable<IFilesDb> dbs)
        {
            var dbName = configuration.GetConnectionStringOrThrow("db");
            log.LogInformation($"use db '{dbName}' as {typeof(IFilesDb)}");
            return dbs.FirstOrDefault(db => db.Name == dbName && dbName != Name)
                ?? throw new PhysException($"files db '{dbName}' not found");
        }
    }
}
