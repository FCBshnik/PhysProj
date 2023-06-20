using Phys.Lib.Core.Localization;

namespace Phys.Lib.Core.Authors
{
    public class AuthorDbo
    {
        public string Id { get; set; }

        public string Code { get; set; }

        public string Born { get; set; }

        public string Died { get; set; }

        public List<NameDbo> Names { get; set; } = new List<NameDbo>();
    }
}
