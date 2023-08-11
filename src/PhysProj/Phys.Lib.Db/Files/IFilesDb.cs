namespace Phys.Lib.Db.Files
{
    public interface IFilesDb
    {
        List<FileDbo> Find(FilesDbQuery query);

        void Create(FileDbo file);

        void Update(string code, FileDbUpdate update);

        void Delete(string code);
    }
}
