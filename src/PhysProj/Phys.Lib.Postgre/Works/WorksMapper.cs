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
                Infos = work.Infos.Values.Select(Map).ToList(),
                AuthorsCodes = work.Authors.Values.Select(a => a.AuthorCode).ToList(),
                SubWorksCodes = work.SubWorks.Values.Select(w => w.SubWorkCode).ToList(),
                FilesCodes = work.Files.Values.Select(f => f.FileCode).ToList(),
                SubWorksAuthorsCodes = work.SubWorksAuthors.Values.Select(a => a.AuthorCode).ToList(),
                IsPublic = work.IsPublic,
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
