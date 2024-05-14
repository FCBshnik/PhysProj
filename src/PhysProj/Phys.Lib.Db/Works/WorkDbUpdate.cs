namespace Phys.Lib.Db.Works
{
    public class WorkDbUpdate
    {
        public string? Publish { get; set; }

        public string? Language { get; set; }

        public WorkDbo.InfoDbo? AddInfo { get; set; }

        /// <summary>
        /// Deletes info for specified language
        /// </summary>
        public string? DeleteInfo { get; set; }

        public string? AddAuthor { get; set; }

        public string? DeleteAuthor { get; set; }

        public string? AddSubWork { get; set; }

        public string? DeleteSubWork { get; set; }

        public string? AddSubWorkAuthor { get; set; }

        public string? DeleteSubWorkAuthor { get; set; }

        public string? AddFile { get; set; }

        public string? DeleteFile { get; set; }

        public bool? IsPublic { get; set; }
    }
}
