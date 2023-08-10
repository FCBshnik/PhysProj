namespace Phys.Lib.Db.Files
{
    public class FilesDbQuery
    {
        public string? Id { get; set; }

        public string? Code { get; set; }

        public string? Search { get; set; }

        public int Limit { get; set; } = 20;
    }
}
