using Riok.Mapperly.Abstractions;
using Phys.Lib.Db.Works;

namespace Phys.Lib.Admin.Api.Api.Works
{
    [Mapper]
    public partial class WorksMapper
    {
        public partial WorkModel Map(WorkDbo work);

        public WorkDbUpdate Map(WorkUpdateModel model)
        {
            return new WorkDbUpdate
            {
                Publish = model.Date,
                Language = model.Language,
            };
        }

        public WorkDbo.InfoDbo Map(WorkInfoUpdateModel model, string language)
        {
            return new WorkDbo.InfoDbo
            {
                Language = language,
                Name = model.Name,
                Description = model.Description,
            };
        }
    }
}
