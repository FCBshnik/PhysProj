using Phys.Lib.Core.Files;

namespace Phys.Lib.Admin.Api.Api.Files
{
    internal static class FilesMapper
    {
        public static FileLinksModel Map(FileDbo file)
        {
            return new FileLinksModel
            {
                Id = file.Id,
                Code = file.Code,
                Format = file.Format,
                Size = file.Size,
                Links = file.Links.ConvertAll(l => new FileLinksModel.LinkModel { Type = l.Type, Path = l.Path }),
            };
        }

        public static FileStorageModel Map(FileStorageDbo storage)
        {
            return new FileStorageModel
            {
                Code = storage.Code,
                Name = storage.Name,
            };
        }

        public static FileStorageFileInfoModel Map(Lib.Files.FileInfo info)
        {
            return new FileStorageFileInfoModel
            {
                Path = info.Path,
                Size = info.Size,
                Updated = info.Updated,
            };
        }
    }
}
