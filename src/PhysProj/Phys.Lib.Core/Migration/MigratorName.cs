namespace Phys.Lib.Core.Migration
{
    internal static class MigratorName
    {
        public static readonly string Users = "users";
        public static readonly string Authors = "authors";
        public static readonly string Files = "files";
        public static readonly string Works = "works";

        /// <summary>
        /// Aggregation migrator which aggregates authors, files links and works.
        /// </summary>
        public static readonly string Library = "library";

        public static readonly string FilesContent = "files-content";

        public static readonly string WorksSearch = "works-search";
    }
}
