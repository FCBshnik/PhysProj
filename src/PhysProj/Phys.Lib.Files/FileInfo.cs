namespace Phys.Lib.Files
{
    public class FileInfo
    {
        public required string Path { get; set; }

        public DateTime? Updated { get; set; }

        public long? Size { get; set; }

        public override string ToString()
        {
            return $"{Path}";
        }
    }
}