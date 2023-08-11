using Phys.Lib.Db.Works;

namespace Phys.Lib.Postgres.Works
{
    internal static class WorksMapper
    {
        public static WorkDbo Map(WorkModel work)
        {
            return new WorkDbo
            {
                Code = work.Code,
                Language = work.Language,
                Publish = work.Publish,
                OriginalCode = work.OriginalCode,
                Infos = work.Infos.Select(Map).ToList(),
                AuthorsCodes = work.Authors.Select(a => a.AuthorCode).ToList(),
                SubWorksCodes = work.SubWorks.Select(w => w.SubWorkCode).ToList(),
                FilesCodes = work.Files.Select(f => f.FileCode).ToList(),
            };
        }

        public static WorkDbo.InfoDbo Map(WorkModel.InfoModel info)
        {
            return new WorkDbo.InfoDbo
            {
                Language = info.Language,
                Name = info.Name,
                Description = info.Description,
            };
        }
    }
}
