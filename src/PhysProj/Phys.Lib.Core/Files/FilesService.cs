using NLog;

namespace Phys.Lib.Core.Files
{
    internal class FilesService : IFilesService
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly IFilesDb db;
        private readonly IFileStoragesService fileStoragesService;

        public FilesService(IFileStoragesService fileStoragesService, IFilesDb db)
        {
            ArgumentNullException.ThrowIfNull(fileStoragesService);
            ArgumentNullException.ThrowIfNull(db);

            this.fileStoragesService = fileStoragesService;
            this.db = db;
        }

        public FileDbo CreateFromStorageFile(string storageCode, string filePath)
        {
            ArgumentNullException.ThrowIfNull(storageCode);
            ArgumentNullException.ThrowIfNull(filePath);

            var storage = fileStoragesService.Get(storageCode);
            var storageFile = storage.Get(filePath);
            var code = Code.NormalizeAndValidate(filePath);
            var file = db.Create(new FileDbo
            {
                Code = code,
                Format = Path.GetExtension(storageFile.Path),
                Size = storageFile.Size,
                Links = new List<FileDbo.LinkDbo>
                {
                    new FileDbo.LinkDbo { Type = storageCode, Path = storageFile.Path }
                },
            });

            log.Info($"created file {file}");
            return file;
        }

        public List<FileDbo> Find(string? search = null)
        {
            return db.Find(new FileLinksDbQuery { Search = search });
        }

        public FileDbo? FindByCode(string code)
        {
            ArgumentNullException.ThrowIfNull(code);

            return db.Find(new FileLinksDbQuery { Code = code }).FirstOrDefault();
        }

        public void Delete(FileDbo file)
        {
            ArgumentNullException.ThrowIfNull(file);

            db.Delete(file.Id);

            log.Info($"deletd file '{file}'");
        }
    }
}
