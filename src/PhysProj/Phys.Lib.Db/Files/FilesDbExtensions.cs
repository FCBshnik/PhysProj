namespace Phys.Lib.Db.Files
{
    public static class FilesDbExtensions
    {
        public static FileDbo Get(this IFilesDb db, string id)
        {
            ArgumentNullException.ThrowIfNull(db);
            ArgumentNullException.ThrowIfNull(id);

            var files = db.Find(new FilesDbQuery { Id = id });
            if (files.Count == 1)
                return files[0];

            throw new ApplicationException($"failed get file with id '{id}' from '{db.GetType().FullName}' due to found {files.Count} files");
        }

        public static FileDbo GetByCode(this IFilesDb db, string code)
        {
            ArgumentNullException.ThrowIfNull(db);
            ArgumentNullException.ThrowIfNull(code);

            var files = db.Find(new FilesDbQuery { Code = code });
            if (files.Count == 1)
                return files[0];

            throw new ApplicationException($"failed get file with code '{code}' from '{db.GetType().FullName}' due to found {files.Count} files");
        }
    }
}
