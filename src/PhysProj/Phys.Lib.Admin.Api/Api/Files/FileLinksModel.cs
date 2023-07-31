namespace Phys.Lib.Admin.Api.Api.Files
{
    public class FileLinksModel
    {
        public string Id { get; set; }

        public string Code { get; set; }

        public string? Format { get; set; }

        public long? Size { get; set; }

        public List<LinkModel> Links { get; set; }

        public class LinkModel
        {
            public string Type { get; set; }

            public string Path { get; set; }
        }
    }
}
