using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Phys.Lib.Db.Files;
using Phys.Shared;

namespace Phys.Lib.Core.Files
{
    internal class FilesDbs : IFilesDbs
    {
        private readonly IConfiguration configuration;
        private readonly ILogger<FilesDbs> log;
        private readonly Lazy<IFilesDb> db;

        public string Name => "main";

        public FilesDbs(IEnumerable<IFilesDb> dbs, IConfiguration configuration, ILogger<FilesDbs> log)
        {
            this.configuration = configuration;
            this.log = log;

            db = new Lazy<IFilesDb>(() => GetDb(dbs));
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
            var dbName = configuration.GetConnectionString("db");
            if (dbName == null)
                throw new PhysException($"connection string 'db' is missed");

            log.LogInformation($"use db '{dbName}' as {typeof(IFilesDb)}");
            return dbs.FirstOrDefault(db => db.Name == dbName && dbName != Name)
                ?? throw new PhysException($"files db '{dbName}' not found");
        }
    }
}
