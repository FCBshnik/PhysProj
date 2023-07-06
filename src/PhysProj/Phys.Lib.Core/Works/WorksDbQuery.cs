namespace Phys.Lib.Core.Works
{
    public class WorksDbQuery
    {
        public string? Search { get; set; }

        public string? Code { get; set; }

        public string? AuthorCode { get; set; }

        public int Limit { get; set; } = 20;
    }
}
