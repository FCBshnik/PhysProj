namespace Phys.Lib.Admin.Api.Api.Files
{
    public class FileModel
    {
        public string Code { get; set; }

        public string? Format { get; set; }

        public long Size { get; set; }

        public List<LinkModel> Links { get; set; }

        public class LinkModel
        {
            public string StorageCode { get; set; }

            public string FileId { get; set; }
        }
    }
}
