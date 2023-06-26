namespace Phys.Lib.Core.Authors
{
    public class AuthorDbo
    {
        public string Id { get; set; }

        public string Code { get; set; }

        public string Born { get; set; }

        public string Died { get; set; }

        public List<InfoDbo> Infos { get; set; } = new List<InfoDbo>();

        public class InfoDbo
        {
            public string Language { get; set; }

            public string Name { get; set; }

            public string Description { get; set; }
        }

        public override string ToString()
        {
            return $"{Code} ({Id})";
        }
    }
}
