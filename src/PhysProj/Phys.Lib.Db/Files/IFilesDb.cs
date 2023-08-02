namespace Phys.Lib.Db.Files
{
    public interface IFilesDb
    {
        List<FileDbo> Find(FilesDbQuery query);

        FileDbo Create(FileDbo file);

        FileDbo Update(string id, FileDbUpdate update);

        void Delete(string id);
    }
}
