namespace Phys.Lib.Admin.Api.Api.User
{
    public class UserModel
    {
        public required string Name { get; set; }

        public required List<string> Roles { get; set; }
    }
}
