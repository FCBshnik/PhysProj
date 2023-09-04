namespace Phys.Lib.Db.Files
{
    public static class FilesDbExtensions
    {
        public static FileDbo GetByCode(this IFilesDb db, string code)
        {
            ArgumentNullException.ThrowIfNull(db);
            ArgumentNullException.ThrowIfNull(code);

            var files = db.Find(new FilesDbQuery { Code = code });
            if (files.Count == 1)
                return files[0];

            throw new PhysDbException($"failed get file with code '{code}' from '{db.GetType().FullName}' due to found {files.Count} files");
        }
    }
}
