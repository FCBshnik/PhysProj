namespace Phys.Lib.Admin.Api.Api.Files
{
    public class FileStorageFileInfoModel
    {
        public required string Path { get; set; }

        public DateTime? Updated { get; set; }

        public long? Size { get; set; }
    }
}
