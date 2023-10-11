using Phys.Lib.Db.Files;

namespace Phys.Lib.Mongo.Files
{
    internal static class FileMapper
    {
        public static FileDbo Map(FileModel file)
        {
            return new FileDbo
            {
                Code = file.Code,
                Format = file.Format,
                Size = file.Size,
                Links = file.Links.Select(Map).ToList(),
            };
        }

        public static FileDbo.LinkDbo Map(FileModel.LinkModel link)
        {
            return new FileDbo.LinkDbo
            {
                StorageCode = link.Type,
                FileId = link.Path,
            };
        }

        public static FileModel.LinkModel Map(FileDbo.LinkDbo link)
        {
            return new FileModel.LinkModel
            {
                Type = link.StorageCode,
                Path = link.FileId,
            };
        }
    }
}
