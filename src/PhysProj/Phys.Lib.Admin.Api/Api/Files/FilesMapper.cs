using Phys.Lib.Core.Files;

namespace Phys.Lib.Admin.Api.Api.Files
{
    internal static class FilesMapper
    {
        public static FileStorageModel Map(FileStorageDbo storage)
        {
            return new FileStorageModel
            {
                Code = storage.Code,
                Name = storage.Name,
            };
        }

        public static FileStorageFileModel Map(Lib.Files.FileInfo info)
        {
            return new FileStorageFileModel
            {
                Path = info.Path,
                Size = info.Size,
                Updated = info.Updated,
            };
        }
    }
}
