using Riok.Mapperly.Abstractions;
using Phys.Lib.Db.Authors;

namespace Phys.Lib.Admin.Api.Api.Authors
{
    [Mapper]
    public partial class AuthorsMapper
    {
        public partial AuthorModel Map(AuthorDbo author);
    }
}
