using NLog;
using Phys.Lib.Base.Files;

namespace Phys.Lib.Files.Local
{
    public class SystemFileStorage : IFileStorage
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly DirectoryInfo baseDir;

        public SystemFileStorage(string code, string baseDir)
        {
            ArgumentException.ThrowIfNullOrEmpty(code);
            ArgumentException.ThrowIfNullOrEmpty(baseDir);

            Code = code;
            this.baseDir = new DirectoryInfo(baseDir);
            log.Info($"base dir '{this.baseDir.FullName}'");
        }

        public string Code { get; }

        public string Name => "System file storage";

        public void Delete(string path)
        {
            ArgumentNullException.ThrowIfNull(path);

            log.Info($"deleting '{path}'");

            var fileInfo = GetFileInfo(path);
            if (fileInfo.Exists)
            {
                log.Info($"deleting '{fileInfo.FullName}'");
                fileInfo.Delete();
                log.Info($"deleted '{fileInfo.FullName}'");
            }
        }

        public Stream Download(string path)
        {
            ArgumentNullException.ThrowIfNull(path);

            var fileInfo = GetFileInfo(path);
            return fileInfo.OpenRead();
        }

        public StorageFileInfo? Get(string path)
        {
            ArgumentNullException.ThrowIfNull(path);

            var fileInfo = GetFileInfo(path);
            if (!fileInfo.Exists)
                return null;

            return MapFileInfo(fileInfo);
        }

        public List<StorageFileInfo> List(string? search)
        {
            if (!Directory.Exists(baseDir.FullName))
                return Enumerable.Empty<StorageFileInfo>().ToList();

            return baseDir.EnumerateFiles(search != null ? $"*{search}*" : "*", SearchOption.AllDirectories)
                .Take(100)
                .Select(MapFileInfo)
                .ToList();
        }

        public StorageFileInfo Upload(string path, Stream data)
        {
            ArgumentNullException.ThrowIfNull(path);
            ArgumentNullException.ThrowIfNull(data);

            var fileInfo = GetFileInfo(path);
            if (fileInfo.Directory?.Exists == false)
                fileInfo.Directory.Create();
            using var fileStream = File.OpenWrite(fileInfo.FullName);
            data.CopyTo(fileStream);
            return MapFileInfo(GetFileInfo(path));
        }

        private StorageFileInfo MapFileInfo(FileInfo f)
        {
            return new StorageFileInfo { Path = GetFilePath(f), Size = f.Length, Updated = f.LastWriteTimeUtc };
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
