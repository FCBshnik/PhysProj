using Microsoft.Extensions.Logging;
using Phys.Files;
using Phys.Lib.Core.Migration;
using Phys.Lib.Db.Files;

namespace Phys.Lib.Core.Files.Storage
{
    internal class FilesContentMigrator : IMigrator
    {
        private readonly ILogger<FilesContentMigrator> log;
        private readonly Dictionary<string, IFileStorage> storages;
        private readonly IFilesDb filesDb;

        public FilesContentMigrator(IEnumerable<IFileStorage> storages, ILogger<FilesContentMigrator> log, IFilesDb filesDb)
        {
            ArgumentNullException.ThrowIfNull(storages);
            ArgumentNullException.ThrowIfNull(log);

            this.storages = storages.ToDictionary(s => s.Code, s => s, StringComparer.OrdinalIgnoreCase);
            this.log = log;
            this.filesDb = filesDb;
        }

        public IEnumerable<string> Sources => storages.Keys;

        public IEnumerable<string> Destinations => storages.Keys;

        public string Name => MigratorName.FilesContent;

        public void Migrate(MigrationDto migration, IProgress<MigrationDto> progress)
        {
            var src = storages[migration.Source];
            var dst = storages[migration.Destination];

            foreach (var files in filesDb.Read(100))
            {
                foreach (var file in files)
                {
                    var srcLink = file.Links.Find(l => l.StorageCode == src.Code);
                    var dstLink = file.Links.Find(l => l.StorageCode == dst.Code);
                    if (srcLink == null)
                        continue;

                    if (dstLink != null)
                    {
                        migration.Stats.Skipped++;
                        continue;
                    }

                    var srcFileInfo = src.Get(srcLink.FileId);
                    if (srcFileInfo == null)
                    {
                        log.LogError($"file '{srcLink.FileId}' not found in '{src.Code}' storage");
                        migration.Stats.Failed++;
                        continue;
                    }

                    log.LogInformation($"copying file '{srcFileInfo.Name}' size {srcFileInfo.Size} bytes");
                    using (var data = src.Download(srcLink.FileId))
                    {
                        var dstFileInfo = dst.Upload(data, srcFileInfo.Name);
                        log.LogInformation($"copied file '{srcFileInfo.Name}' size {srcFileInfo.Size} bytes");
                        filesDb.Update(file.Code, new FileDbUpdate { AddLink = new FileDbo.LinkDbo { StorageCode = dst.Code, FileId = dstFileInfo.Id } });
                        migration.Stats.Created++;
                        progress.Report(migration);
                    }
                }
            }
        }
    }
}
