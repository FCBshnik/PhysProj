using Phys.Lib.Core.Files.Storage;
using Phys.Lib.Db.Files;
using Phys.Files;

namespace Phys.Lib.Admin.Api.Api.Files
{
    internal static class FilesMapper
    {
        public static FileModel Map(FileDbo file)
        {
            return new FileModel
            {
                Code = file.Code,
                Format = file.Format,
                Size = file.Size,
                Links = file.Links.ConvertAll(l => new FileModel.LinkModel { StorageCode = l.StorageCode, FileId = l.FileId }),
            };
        }

        public static FileStorageModel Map(FileStorageInfo storage)
        {
            return new FileStorageModel
            {
                Code = storage.Code,
                Name = storage.Name,
            };
        }

        public static FileStorageFileModel Map(StorageFileInfo info)
        {
            return new FileStorageFileModel
            {
                FileId = info.Id,
                Name = info.Name,
                Size = info.Size,
                Updated = info.Updated,
            };
        }
    }
}
