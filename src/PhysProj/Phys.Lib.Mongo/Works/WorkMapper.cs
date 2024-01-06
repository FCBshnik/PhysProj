using Phys.Lib.Db.Works;

namespace Phys.Lib.Mongo.Works
{
    internal static class WorkMapper
    {
        public static WorkDbo Map(WorkModel work)
        {
            return new WorkDbo
            {
                Code = work.Code,
                Language = work.Language,
                Publish = work.Publish,
                AuthorsCodes = work.AuthorsCodes,
                FilesCodes = work.FilesCodes,
                OriginalCode = work.OriginalCode,
                SubWorksCodes = work.SubWorksCodes,
                Infos = work.Infos.Select(Map).ToList(),
                IsPublic = work.IsPublic,
            };
        }

        public static WorkDbo.InfoDbo Map(WorkModel.InfoModel info)
        {
            return new WorkDbo.InfoDbo
            {
                Language = info.Language,
                Description = info.Description,
                Name = info.Name,
            };
        }

        public static WorkModel.InfoModel Map(WorkDbo.InfoDbo info)
        {
            return new WorkModel.InfoModel
            {
                Language = info.Language,
                Description = info.Description,
                Name = info.Name,
            };
        }
    }
}
