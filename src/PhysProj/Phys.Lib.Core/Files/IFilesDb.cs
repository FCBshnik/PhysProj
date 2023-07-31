namespace Phys.Lib.Core.Files
{
    public interface IFilesDb
    {
        List<FileDbo> Find(FileLinksDbQuery query);

        FileDbo Create(FileDbo file);

        FileDbo Update(string id, FileDbUpdate update);

        void Delete(string id);
    }
}
