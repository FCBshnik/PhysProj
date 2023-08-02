namespace Phys.Lib.Db.Users
{
    public interface IUsersDb
    {
        UserDbo Create(UserDbo user);

        List<UserDbo> Find(UsersDbQuery query);

        UserDbo Update(string id, UserDbUpdate update);
    }
}
