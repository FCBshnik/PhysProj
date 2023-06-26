using Phys.Lib.Core.Authors;
using Riok.Mapperly.Abstractions;

namespace Phys.Lib.Api.Admin.Api.Authors
{
    [Mapper]
    public partial class AuthorsMapper
    {
        public partial AuthorModel Map(AuthorDbo author);
    }
}
