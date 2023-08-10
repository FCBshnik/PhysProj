namespace Phys.Lib.Db.Files
{
    public static class FilesDbExtensions
    {
        public static FileDbo Get(this IFilesDb db, string id)
        {
            ArgumentNullException.ThrowIfNull(db);
            ArgumentNullException.ThrowIfNull(id);

            return db.Find(new FilesDbQuery { Id = id }).FirstOrDefault() ?? throw new ApplicationException($"author '{id}' not found in '{db.GetType().FullName}'");
        }
    }
}
