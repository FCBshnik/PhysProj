namespace Phys.Lib.Core.Files
{
    public class FileLinksDbQuery
    {
        public string? Code { get; set; }

        public string? Search { get; set; }

        public int Limit { get; set; } = 20;
    }
}
