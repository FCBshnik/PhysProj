namespace Phys.Lib.Core.Files
{
    public class FileDbo
    {
        public string Id { get; set; }

        public string Code { get; set; }

        public string Format { get; set; }

        public long Size { get; set; }

        public List<LinkDbo> Links { get; set; } = new List<LinkDbo>();

        public class LinkDbo
        {
            public string Type { get; set; }

            public string Path { get; set; }
        }
    }
}
