namespace Phys.Files
{
    public class StorageFileInfo
    {
        public required string Id { get; set; }

        public required string Name { get; set; }

        public required long Size { get; set; }

        public DateTime? Updated { get; set; }

        public override string ToString()
        {
            return $"{Id}";
        }
    }
}
