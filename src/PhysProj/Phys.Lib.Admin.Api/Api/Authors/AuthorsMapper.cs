using Phys.Lib.Core.Authors;
using Riok.Mapperly.Abstractions;
using Phys.Lib.Admin.Api.Api.Authors;

namespace Phys.Lib.Admin.Api.Api.Authors
{
    [Mapper]
    public partial class AuthorsMapper
    {
        public partial AuthorModel Map(AuthorDbo author);
    }
}
