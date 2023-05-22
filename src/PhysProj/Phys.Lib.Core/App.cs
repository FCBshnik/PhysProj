using Phys.Lib.Core.Users;

namespace Phys.Lib.Core
{
    public class App
    {
        public App(IUsers users)
        {
            Users = users;
        }

        public IUsers Users { get; }
    }
}
