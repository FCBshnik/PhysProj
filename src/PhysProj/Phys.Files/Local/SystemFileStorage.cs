using Microsoft.Extensions.Logging;

namespace Phys.Files.Local
{
    public class SystemFileStorage : IFileStorage
    {
        private readonly ILogger<SystemFileStorage> log;
        private readonly DirectoryInfo baseDir;

        public SystemFileStorage(string code, string baseDir, ILogger<SystemFileStorage> log)
        {
            ArgumentException.ThrowIfNullOrEmpty(code);
            ArgumentException.ThrowIfNullOrEmpty(baseDir);

            Code = code;
            this.baseDir = new DirectoryInfo(baseDir);
            this.log = log;

            log.Log(LogLevel.Information, $"base dir '{this.baseDir.FullName}'");
        }

        public string Code { get; }

        public string Name => "System file storage";

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

            return baseDir.EnumerateFiles(search != null ? $"*{search}*" : "*", SearchOption.AllDirectories)
                .Take(100)
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
            return MapFileInfo(GetFileInfo(name));
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
