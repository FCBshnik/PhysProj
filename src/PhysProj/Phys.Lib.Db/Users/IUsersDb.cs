namespace Phys.Lib.Db.Users
{
    public interface IUsersDb
    {
        void Create(UserDbo user);

        List<UserDbo> Find(UsersDbQuery query);

        void Update(string nameLowerCase, UserDbUpdate update);
    }
}
