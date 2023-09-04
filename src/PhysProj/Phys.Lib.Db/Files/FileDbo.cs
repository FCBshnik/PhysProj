namespace Phys.Lib.Db.Files
{
    public class FileDbo
    {
        public required string Code { get; set; }

        public string? Format { get; set; }

        public long? Size { get; set; }

        public List<LinkDbo> Links { get; set; } = new List<LinkDbo>();

        public class LinkDbo
        {
            public required string Type { get; set; }

            public required string Path { get; set; }

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
