using Generator.Equals;

namespace Phys.Lib.Db.Files
{
    /// <summary>
    /// File metadata with collection of links where content of file is stored
    /// </summary>
    [Equatable]
    public sealed partial class FileDbo
    {
        /// <summary>
        /// Semantic and url-compatible id of file
        /// </summary>
        public required string Code { get; set; }

        /// <summary>
        /// Format of file
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// Size of file in bytes
        /// </summary>
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
