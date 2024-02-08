namespace Phys.Lib.Db.Users
{
    public interface IUsersDb : IDbReader<UserDbo>
    {
        void Create(UserDbo user);

        List<UserDbo> Find(UsersDbQuery query);

        void Update(string name, UserDbUpdate update);
    }
}
