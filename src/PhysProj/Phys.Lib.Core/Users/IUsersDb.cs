namespace Phys.Lib.Core.Users
{
    public interface IUsersDb
    {
        UserDbo Create(UserDbo user);

        UserDbo Get(string id);

        List<UserDbo> Find(UsersQuery query);
    }
}
