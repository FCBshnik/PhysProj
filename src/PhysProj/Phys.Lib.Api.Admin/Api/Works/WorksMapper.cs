using Phys.Lib.Core.Works;
using Riok.Mapperly.Abstractions;

namespace Phys.Lib.Api.Admin.Api.Works
{
    [Mapper]
    public partial class WorksMapper
    {
        public partial WorkModel Map(WorkDbo work);

        [MapperIgnoreTarget(nameof(WorkUpdate.AddInfo))]
        [MapperIgnoreTarget(nameof(WorkUpdate.DeleteInfo))]
        [MapperIgnoreTarget(nameof(WorkUpdate.AddAuthorId))]
        [MapperIgnoreTarget(nameof(WorkUpdate.DeleteAuthorId))]
        [MapperIgnoreTarget(nameof(WorkUpdate.AddOriginalId))]
        [MapperIgnoreTarget(nameof(WorkUpdate.DeleteOriginalId))]
        public partial WorkUpdate Map(WorkUpdateModel model);

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
