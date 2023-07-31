namespace Phys.Lib.Core.Files
{
    public interface IFilesLinksDb
    {
        List<FileLinksDbo> Find(FileLinksDbQuery query);

        FileLinksDbo Create(FileLinksDbo file);

        FileLinksDbo Update(string id, FileLinksDbUpdate update);

        void Delete(string id);
    }
}
