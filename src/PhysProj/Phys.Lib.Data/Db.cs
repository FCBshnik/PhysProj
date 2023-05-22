using MongoDB.Bson;
using Phys.Lib.Core;
using Phys.Lib.Core.Users;

namespace Phys.Lib.Data
{
    public class Db : IDb
    {
        public string NewId()
        {
            return ObjectId.GenerateNewId().ToString();
        }

        public IUsersDb Users { get; }

        public Db(IUsersDb users)
        {
            Users = users;
        }
    }
}
