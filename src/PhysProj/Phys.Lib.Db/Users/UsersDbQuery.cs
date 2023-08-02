namespace Phys.Lib.Db.Users
{
    public class UsersDbQuery
    {
        public string? NameLowerCase { get; set; }

        public string? Code { get; set; }

        public int Limit { get; set; } = 20;
    }
}
