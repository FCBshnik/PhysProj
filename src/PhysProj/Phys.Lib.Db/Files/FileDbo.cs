namespace Phys.Lib.Db.Files
{
    public class FileDbo
    {
        public string Id { get; set; }

        public string Code { get; set; }

        public string? Format { get; set; }

        public long? Size { get; set; }

        public List<LinkDbo> Links { get; set; } = new List<LinkDbo>();

        public class LinkDbo
        {
            public string Type { get; set; }

            public string Path { get; set; }

            public override string ToString()
            {
                return $"{Path} ({Type})";
            }
        }

        public override string ToString()
        {
            return $"{Code} ({Format}, {Size})";
        }
    }
}
