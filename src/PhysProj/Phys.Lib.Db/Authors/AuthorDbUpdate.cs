namespace Phys.Lib.Db.Authors
{
    public class AuthorDbUpdate
    {
        public string? Born { get; set; }

        public string? Died { get; set; }

        public AuthorDbo.InfoDbo? AddInfo { get; set; }

        public string? DeleteInfo { get; set; }
    }
}
