using NLog;
using Phys.Lib.Db.Files;

namespace Phys.Lib.Core.Files
{
    internal class FilesService : IFilesService
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly IFilesDb db;

        public FilesService(IFilesDb db)
        {
            ArgumentNullException.ThrowIfNull(db);

            this.db = db;
        }

        public FileDbo Create(string code, string? format, long? size)
        {
            code = Code.NormalizeAndValidate(code);
            var file = db.Create(new FileDbo
            {
                Code = code,
                Format = format,
                Size = size,
            });

            log.Info($"created file {file}");
            return file;
        }

        public FileDbo AddLink(FileDbo file, FileDbo.LinkDbo link)
        {
            ArgumentNullException.ThrowIfNull(file);
            ArgumentNullException.ThrowIfNull(link);
            ArgumentNullException.ThrowIfNull(link.Path);
            ArgumentNullException.ThrowIfNull(link.Type);

            var update = new FileDbUpdate { AddLink = link };
            file = db.Update(file.Id, update);
            log.Info($"file {file} link added: {link}");
            return file;
        }

        public List<FileDbo> Find(string? search = null)
        {
            return db.Find(new FilesDbQuery { Search = search });
        }

        public FileDbo? FindByCode(string code)
        {
            ArgumentNullException.ThrowIfNull(code);

            return db.Find(new FilesDbQuery { Code = code }).FirstOrDefault();
        }

        public void Delete(FileDbo file)
        {
            ArgumentNullException.ThrowIfNull(file);

            db.Delete(file.Id);

            log.Info($"deletd file '{file}'");
        }
    }
}
