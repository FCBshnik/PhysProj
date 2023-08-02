using Phys.Lib.Db.Users;

namespace Phys.Lib.Core.Users
{
    public interface IUsersService
    {
        UserDbo Create(string name, string password);

        UserDbo AddRole(UserDbo user, UserRole role);

        UserDbo DeleteRole(UserDbo user, UserRole role);

        Result<UserDbo> Login(string name, string password, UserRole role);

        UserDbo GetByName(string name);
    }
}
