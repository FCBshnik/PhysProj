using Phys.Lib.Core.Users;
using Phys.Lib.Core.Validation;

namespace Phys.Lib.Core
{
    public class App
    {
        public App(IUsers users, IValidation validator)
        {
            Users = users;
            Validator = validator;
        }

        public IUsers Users { get; }

        public IValidation Validator { get; }
    }
}
