namespace Phys.Lib.Core
{
    public static class EventNames
    {
        public static readonly string WorksCacheInvalidated = "works-cache-invalidated";
        public static readonly string AuthorsCacheInvalidated = "authors-cache-invalidated";
        public static readonly string FilesCacheInvalidated = "files-cache-invalidated";

        public static readonly string WorkCreated = "works-work-created";
        public static readonly string WorkUpdated = "works-work-updated";
        public static readonly string WorkDeleted = "works-work-deleted";

        public static readonly string AuthorCreated = "authors-author-created";
        public static readonly string AuthorUpdated = "authors-author-updated";
        public static readonly string AuthorDeleted = "authors-author-deleted";

        public static readonly string FileCreated = "files-file-created";
        public static readonly string FileUpdated = "files-file-updated";
        public static readonly string FileDeleted = "files-file-deleted";
    }
}
