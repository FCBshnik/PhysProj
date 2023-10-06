namespace Phys.Files
{
    public class StorageFileInfo
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
