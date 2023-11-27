namespace Phys.Lib.Search
{
    public class WorkTso
    {
        public required string Code { get; set; }

        public Dictionary<string, string?> Names { get; set; } = new Dictionary<string, string?>();

        public Dictionary<string, List<string?>> Authors { get; set; } = new Dictionary<string, List<string?>>();
    }
}
