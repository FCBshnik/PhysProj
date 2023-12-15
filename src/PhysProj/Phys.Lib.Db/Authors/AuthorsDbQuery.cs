namespace Phys.Lib.Db.Authors
{
    public class AuthorsDbQuery
    {
        public string? Search { get; set; }

        public string? Code { get; set; }

        public List<string>? Codes { get; set; }

        public int Limit { get; set; } = 1000;
    }
}
