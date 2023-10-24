using Generator.Equals;

namespace Phys.Lib.Db.Files
{
    [Equatable]
    public sealed partial class FileDbo
    {
        public required string Code { get; set; }

        public string? Format { get; set; }

        public long Size { get; set; }

        [UnorderedEquality]
        public List<LinkDbo> Links { get; set; } = new List<LinkDbo>();

        [Equatable]
        public sealed partial class LinkDbo
        {
            public required string StorageCode { get; set; }

            public required string FileId { get; set; }

            public override string ToString()
            {
                return $"{FileId} ({StorageCode})";
            }
        }

        public override string ToString()
        {
            return $"{Code} ({Format}, {Size})";
        }
    }
}
