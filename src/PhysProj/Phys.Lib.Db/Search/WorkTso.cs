namespace Phys.Lib.Db.Search
{
    public class WorkTso
    {
        public string Code { get; set; }

        public Dictionary<string, string?> Names { get; set; } = new Dictionary<string, string?>();
    }
}
