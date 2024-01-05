namespace Phys.Lib.Admin.Api.Api.Files
{
    public class FileModel
    {
        public required string Code { get; set; }

        public required string Format { get; set; }

        public long Size { get; set; }

        public List<LinkModel> Links { get; set; } = new List<LinkModel>();

        public class LinkModel
        {
            public required string StorageCode { get; set; }

            public required string FileId { get; set; }
        }
    }
}
