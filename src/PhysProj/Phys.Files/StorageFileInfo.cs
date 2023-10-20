namespace Phys.Files
{
    public class StorageFileInfo
    {
        /// <summary>
        /// File id unique in storage
        /// </summary>
        public required string Id { get; set; }

        /// <summary>
        /// File name with extension or (optional) full path to file within storage
        /// </summary>
        public required string Name { get; set; }

        public required long Size { get; set; }

        public DateTime? Updated { get; set; }

        public override string ToString()
        {
            return $"{Id}";
        }
    }
}
