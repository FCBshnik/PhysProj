namespace Phys.Lib.Core.Authors
{
    public class AuthorsQuery
    {
        public string? Search { get; set; }

        public string? Code { get; set; }

        public int Limit { get; set; } = 20;
    }
}
