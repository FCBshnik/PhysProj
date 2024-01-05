namespace Phys.Lib.Search
{
    public class AuthorTso
    {
        public required string Code { get; set; }

        public Dictionary<string, string?> Names { get; set; } = new Dictionary<string, string?>();
    }
}
