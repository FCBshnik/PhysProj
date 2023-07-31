using NLog;

namespace Phys.Lib.Core.Files
{
    internal class FileLinksService : IFileLinksService
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly IFilesLinksDb db;
        private readonly IFileStoragesService fileStoragesService;

        public FileLinksService(IFileStoragesService fileStoragesService, IFilesLinksDb db)
        {
            ArgumentNullException.ThrowIfNull(fileStoragesService);
            ArgumentNullException.ThrowIfNull(db);

            this.fileStoragesService = fileStoragesService;
            this.db = db;
        }

        public FileLinksDbo CreateFromStorageFile(string storageCode, string filePath)
        {
            ArgumentNullException.ThrowIfNull(storageCode);
            ArgumentNullException.ThrowIfNull(filePath);

            var storage = fileStoragesService.Get(storageCode);
            var storageFile = storage.Get(filePath);
            var code = Code.NormalizeAndValidate(filePath);
            var fileLinks = db.Create(new FileLinksDbo
            {
                Code = code,
                Format = Path.GetExtension(storageFile.Path),
                Size = storageFile.Size,
                Links = new List<FileLinksDbo.LinkDbo>
                {
                    new FileLinksDbo.LinkDbo { Type = storageCode, Path = storageFile.Path }
                },
            });

            log.Info($"created file links {fileLinks}");

            return fileLinks;
        }

        public List<FileLinksDbo> Find(string? search = null)
        {
            return db.Find(new FileLinksDbQuery { Search = search });
        }
    }
}
