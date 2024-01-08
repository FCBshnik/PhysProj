using Phys.Lib.Db.Works;

namespace Phys.Lib.Core.Works
{
    public interface IWorksEditor
    {
        WorkDbo Create(string code);

        WorkDbo UpdateDate(WorkDbo work, string date);

        WorkDbo UpdateLanguage(WorkDbo work, string language);

        WorkDbo UpdateIsPublic(WorkDbo work, bool isPublic);

        WorkDbo AddInfo(WorkDbo work, WorkDbo.InfoDbo info);

        WorkDbo DeleteInfo(WorkDbo work, string language);

        WorkDbo LinkAuthor(WorkDbo work, string authorCode);

        WorkDbo UnlinkAuthor(WorkDbo work, string authorCode);

        WorkDbo LinkOriginal(WorkDbo work, string originalCode);

        WorkDbo UnlinkOriginal(WorkDbo work);

        WorkDbo LinkWork(WorkDbo work, string workCode);

        WorkDbo UnlinkWork(WorkDbo work, string workCode);

        WorkDbo LinkFile(WorkDbo work, string fileCode);

        WorkDbo UnlinkFile(WorkDbo work, string fileCode);

        void Delete(WorkDbo work);
    }
}
