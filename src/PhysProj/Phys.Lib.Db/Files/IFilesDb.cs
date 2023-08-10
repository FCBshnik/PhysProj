namespace Phys.Lib.Db.Files
{
    public interface IFilesDb
    {
        List<FileDbo> Find(FilesDbQuery query);

        void Create(FileDbo file);

        void Update(string id, FileDbUpdate update);

        void Delete(string id);
    }
}
