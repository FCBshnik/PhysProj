using Phys.Lib.Db.Files;
using Phys.Lib.Db.Migrations;

namespace Phys.Lib.Core.Files
{
    public interface IFilesDbs : IFilesDb
    {
        IDbReader<FileDbo> GetReader();
    }
}
