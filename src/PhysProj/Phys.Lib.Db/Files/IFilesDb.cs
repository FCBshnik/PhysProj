using Phys.Lib.Db.Migrations;
using Phys.Shared;

namespace Phys.Lib.Db.Files
{
    public interface IFilesDb : IDbReader<FileDbo>
    {
        List<FileDbo> Find(FilesDbQuery query);

        void Create(FileDbo file);

        void Update(string code, FileDbUpdate update);

        void Delete(string code);
    }
}
