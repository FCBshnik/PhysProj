using Generator.Equals;

namespace Phys.Lib.Db.Authors
{
    [Equatable]
    public sealed partial class AuthorDbo
    {
        /// <summary>
        /// Semantic and url-compatible id of author
        /// </summary>
        public required string Code { get; set; }

        /// <summary>
        /// Year of birhtday date in internal format
        /// </summary>
        public string? Born { get; set; }

        /// <summary>
        /// Year of death date in internal format
        /// </summary>
        public string? Died { get; set; }

        /// <summary>
        /// Translatable information about author
        /// </summary>
        [UnorderedEquality]
        public List<InfoDbo> Infos { get; set; } = new List<InfoDbo>();

        [Equatable]
        public sealed partial class InfoDbo
        {
            public required string Language { get; set; }

            public string? FullName { get; set; }

            public string? Description { get; set; }
        }

        public override string ToString()
        {
            return $"{Code}";
        }
    }
}
