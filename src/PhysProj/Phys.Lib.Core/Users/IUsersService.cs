namespace Phys.Lib.Core.Users
{
    public interface IUsersService
    {
        UserDbo Create(UserCreate create);

        Result<UserDbo> Login(string userName, string password, UserRole withRole);

        UserDbo? GetByName(string userName);
    }
}
