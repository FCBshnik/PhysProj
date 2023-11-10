using FluentValidation;
using Microsoft.Extensions.Logging;
using Phys.Lib.Core.Files.Storage;
using Phys.Lib.Db.Files;

namespace Phys.Lib.Core.Files
{
    internal class FileDownloadService : IFileDownloadService
    {
        private const string downloadStorage = "pcloud";

        private readonly IFileStorages storages;
        private readonly ILogger<FileDownloadService> logger;

        public FileDownloadService(IFileStorages storages, ILogger<FileDownloadService> logger)
        {
            this.storages = storages;
            this.logger = logger;
        }

        public FileDownloadLink GetDownloadLink(FileDbo file)
        {
            ArgumentNullException.ThrowIfNull(file);

            var link = file.Links.FirstOrDefault(l => l.StorageCode == downloadStorage);
            if (link == null)
            {
                logger.LogInformation($"file {file} has not link in storage {downloadStorage}, can not download");
                throw new ValidationException($"file '{file.Code}' is not available for download");
            }

            var storage = storages.Get(link.StorageCode);
            var url = storage.GetDownloadLink(link.FileId);
            return new FileDownloadLink { Url = url };
        }
    }
}
