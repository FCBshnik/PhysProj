namespace Phys.Lib.Db.Works
{
    public class WorksDbQuery
    {
        public string? Code { get; set; }

        public ICollection<string>? Codes { get; set; }

        public string? Search { get; set; }

        public string? FileCode { get; set; }

        public string? AuthorCode { get; set; }

        public string? SubWorkCode { get; set; }

        public int Limit { get; set; } = 1000;
    }
}
