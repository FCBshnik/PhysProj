using Generator.Equals;

namespace Phys.Lib.Db.Works
{
    [Equatable]
    public sealed partial class WorkDbo
    {
        /// <summary>
        /// Semantic and url-compatible id of work
        /// </summary>
        public required string Code { get; set; }

        /// <summary>
        /// Published date in internal format
        /// </summary>
        public string? Publish { get; set; }

        /// <summary>
        /// Language the work is written on
        /// </summary>
        public string? Language { get; set; }

        /// <summary>
        /// Translatable information about work
        /// </summary>
        [UnorderedEquality]
        public List<InfoDbo> Infos { get; set; } = new List<InfoDbo>();

        /// <summary>
        /// Collection of sub works in collected work or translation[s] in collected work
        /// </summary>
        [UnorderedEquality]
        public List<string> SubWorksCodes { get; set; } = new List<string>();

        /// <summary>
        /// Collection of authors of work
        /// </summary>
        [UnorderedEquality]
        public List<string> AuthorsCodes { get; set; } = new List<string>();

        /// <summary>
        /// Files attachments containing text of book in various formats
        /// </summary>
        [UnorderedEquality]
        public List<string> FilesCodes { get; set; } = new List<string>();

        /// <summary>
        /// Indicates that work is publicly available (present in catalog and searchable) for users
        /// </summary>
        public bool IsPublic { get; set; }

        [Equatable]
        public sealed partial class InfoDbo
        {
            /// <summary>
            /// The language of information text
            /// </summary>
            public required string Language { get; set; }

            /// <summary>
            /// Work's title in specified language
            /// </summary>
            public string? Name { get; set; }

            /// <summary>
            /// Work's description in specified language
            /// </summary>
            public string? Description { get; set; }

            public override string? ToString()
            {
                return $"{Language} ({Name})";
            }
        }

        public override string ToString()
        {
            return $"{Code}";
        }
    }
}
