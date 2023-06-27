namespace Phys.Lib.Core.Works
{
    public class WorksQuery
    {
        public string? Search { get; set; }

        public string? Code { get; set; }

        public int Limit { get; set; } = 20;
    }
}
