using Phys.Lib.Core.Users;

namespace Phys.Lib.Core
{
    public interface IDb
    {
        string NewId();

        IUsersDb Users { get; }
    }
}
