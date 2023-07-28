using NLog;

namespace Phys.Lib.Files.Local
{
    public class SystemFileStorage : IFileStorage
    {
        private static readonly Logger log = LogManager.GetCurrentClassLogger();

        private readonly DirectoryInfo baseDir;

        public SystemFileStorage(string baseDir)
        {
            ArgumentException.ThrowIfNullOrEmpty(baseDir);

            this.baseDir = new DirectoryInfo(baseDir);
        }

        public void Delete(string path)
        {
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
            var fileInfo = GetFileInfo(path);
            return fileInfo.OpenRead();
        }

        public List<FileInfo> List(string? search)
        {
            if (!baseDir.Exists)
                return Enumerable.Empty<FileInfo>().ToList();

            return baseDir.EnumerateFiles(search != null ? $"*{search}*" : "*", SearchOption.AllDirectories)
                .Take(100)
                .Select(MapFileInfo)
                .ToList();
        }

        public FileInfo Upload(string path, Stream data)
        {
            var fileInfo = GetFileInfo(path);
            if (fileInfo.Directory?.Exists == false)
                fileInfo.Directory.Create();
            using var fileStream = File.OpenWrite(fileInfo.FullName);
            data.CopyTo(fileStream);
            return MapFileInfo(GetFileInfo(path));
        }

        private FileInfo MapFileInfo(System.IO.FileInfo f)
        {
            return new FileInfo { Path = GetFilePath(f), Size = f.Length, Updated = f.LastWriteTimeUtc };
        }

        private string GetFilePath(System.IO.FileInfo fileInfo)
        {
            return NormalizePath(fileInfo.FullName.Replace(baseDir.FullName, string.Empty));
        }

        private System.IO.FileInfo GetFileInfo(string path)
        {
            return new System.IO.FileInfo(Path.Combine(baseDir.FullName, path));
        }

        private static string NormalizePath(string path)
        {
            return path.Replace('\\', '/').Replace("//", "/").Trim(' ', '/');
        }
    }
}
