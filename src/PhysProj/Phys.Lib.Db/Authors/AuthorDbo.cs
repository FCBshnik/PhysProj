namespace Phys.Lib.Db.Authors
{
    public class AuthorDbo
    {
        public required string Code { get; set; }

        public string? Born { get; set; }

        public string? Died { get; set; }

        public List<InfoDbo> Infos { get; set; } = new List<InfoDbo>();

        public class InfoDbo
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
