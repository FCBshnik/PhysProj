namespace Phys.Lib.Db.Files
{
    public class FilesDbQuery
    {
        public string? Code { get; set; }

        public string? Search { get; set; }

        public List<string>? Codes { get; set; }

        public int Limit { get; set; } = 1000;
    }
}
