using Phys.Lib.Db.Files;

namespace Phys.Lib.Postgres.Files
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
                Links = file.Links.Values.Select(Map).ToList(),
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
    }
}
