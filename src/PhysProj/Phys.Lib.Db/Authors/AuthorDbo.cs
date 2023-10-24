using Generator.Equals;

namespace Phys.Lib.Db.Authors
{
    [Equatable]
    public sealed partial class AuthorDbo
    {
        public required string Code { get; set; }

        public string? Born { get; set; }

        public string? Died { get; set; }

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
