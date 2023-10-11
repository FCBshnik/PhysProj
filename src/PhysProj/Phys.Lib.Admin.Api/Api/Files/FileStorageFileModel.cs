namespace Phys.Lib.Admin.Api.Api.Files
{
    public class FileStorageFileModel
    {
        public required string FileId { get; set; }

        public required string Name { get; set; }

        public long Size { get; set; }

        public DateTime? Updated { get; set; }
    }
}
