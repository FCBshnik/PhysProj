using Phys.Shared;

namespace Phys.Lib.Db.Users
{
    public interface IUsersDb : INamed
    {
        void Create(UserDbo user);

        List<UserDbo> Find(UsersDbQuery query);

        void Update(string name, UserDbUpdate update);
    }
}
