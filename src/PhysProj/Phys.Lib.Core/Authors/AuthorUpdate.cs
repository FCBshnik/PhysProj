namespace Phys.Lib.Core.Authors
{
    public class AuthorUpdate
    {
        public string? Born { get; set; }

        public string? Died { get; set; }

        public AuthorDbo.InfoDbo? AddInfo { get; set; }

        public string? DeleteInfo { get; set; }
    }
}