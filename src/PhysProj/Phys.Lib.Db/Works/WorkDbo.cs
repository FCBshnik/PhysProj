using Generator.Equals;

namespace Phys.Lib.Db.Works
{
    [Equatable]
    public sealed partial class WorkDbo
    {
        /// <summary>
        /// Unique code of work
        /// </summary>
        public required string Code { get; set; }

        /// <summary>
        /// Published date
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
        /// Collection of sub works in collected work
        /// </summary>
        [UnorderedEquality]
        public List<string> SubWorksCodes { get; set; } = new List<string>();

        /// <summary>
        /// Collection of authors of work
        /// </summary>
        [UnorderedEquality]
        public List<string> AuthorsCodes { get; set; } = new List<string>();

        /// <summary>
        /// Original work translated in this work
        /// </summary>
        public string? OriginalCode { get; set; }

        [UnorderedEquality]
        public List<string> FilesCodes { get; set; } = new List<string>();

        [Equatable]
        public sealed partial class InfoDbo
        {
            public required string Language { get; set; }

            public string? Name { get; set; }

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
