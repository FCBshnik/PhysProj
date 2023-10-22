using Microsoft.Extensions.Logging;
using Phys.Shared.Cache;

namespace Phys.Files.Local
{
    public class LocalFileStorage : IFileStorage
    {
        private readonly ILogger<LocalFileStorage> log;
        private readonly DirectoryInfo baseDir;
        private readonly Expirable<List<FileInfo>> filesCache;

        public LocalFileStorage(string code, DirectoryInfo baseDir, ILogger<LocalFileStorage> log)
        {
            ArgumentException.ThrowIfNullOrEmpty(code);
            ArgumentNullException.ThrowIfNull(baseDir);

            Code = code;
            this.baseDir = baseDir;
            this.log = log;

            log.Log(LogLevel.Information, $"base dir '{this.baseDir.FullName}'");

            filesCache = new Expirable<List<FileInfo>>(ListFiles, TimeSpan.FromDays(1));
        }

        public string Code { get; }

        public void Delete(string fileId)
        {
            ArgumentNullException.ThrowIfNull(fileId);

            log.Log(LogLevel.Information, $"deleting '{fileId}'");

            var fileInfo = GetFileInfo(fileId);
            if (fileInfo.Exists)
            {
                log.Log(LogLevel.Information, $"deleting '{fileInfo.FullName}'");
                fileInfo.Delete();
                log.Log(LogLevel.Information, $"deleted '{fileInfo.FullName}'");
            }

            filesCache.Reset();
        }

        public Stream Download(string fileId)
        {
            ArgumentNullException.ThrowIfNull(fileId);

            var fileInfo = GetFileInfo(fileId);
            return fileInfo.OpenRead();
        }

        public StorageFileInfo? Get(string fileId)
        {
            ArgumentNullException.ThrowIfNull(fileId);

            var fileInfo = GetFileInfo(fileId);
            if (!fileInfo.Exists)
                return null;

            return MapFileInfo(fileInfo);
        }

        public List<StorageFileInfo> List(string? search)
        {
            if (!Directory.Exists(baseDir.FullName))
            {
                log.LogInformation($"dir '{baseDir.FullName}' not exists");
                return Enumerable.Empty<StorageFileInfo>().ToList();
            }

            return filesCache.Value
                .Where(i => search == null || i.Name.Contains(search, StringComparison.OrdinalIgnoreCase))
                .Take(20)
                .Select(MapFileInfo)
                .ToList();
        }

        public StorageFileInfo Upload(Stream data, string name)
        {
            ArgumentNullException.ThrowIfNull(name);
            ArgumentNullException.ThrowIfNull(data);

            var fileInfo = GetFileInfo(name);
            if (fileInfo.Directory?.Exists == false)
                fileInfo.Directory.Create();
            using var fileStream = File.OpenWrite(fileInfo.FullName);
            data.CopyTo(fileStream);
            filesCache.Reset();
            return MapFileInfo(GetFileInfo(name));
        }

        private List<FileInfo> ListFiles()
        {
            var files = baseDir.EnumerateFiles("*", SearchOption.AllDirectories)
                .ToList();

            log.LogInformation($"found {files.Count} files at '{baseDir}'");

            return files;
        }

        private StorageFileInfo MapFileInfo(FileInfo fi)
        {
            return new StorageFileInfo
            {
                Id = GetFilePath(fi),
                Size = fi.Length,
                Updated = fi.LastWriteTimeUtc,
                Name = GetFilePath(fi),
            };
        }

        private string GetFilePath(FileInfo fileInfo)
        {
            return NormalizePath(fileInfo.FullName.Replace(baseDir.FullName, string.Empty));
        }

        private FileInfo GetFileInfo(string path)
        {
            return new FileInfo(Path.Combine(baseDir.FullName, path));
        }

        private static string NormalizePath(string path)
        {
            return path.Replace('\\', '/').Replace("//", "/").Trim(' ', '/');
        }
    }
}
