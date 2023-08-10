namespace Phys.Lib.Db.Users
{
    public class UsersDbQuery
    {
        public string? Id { get; set; }

        public string? NameLowerCase { get; set; }

        public int Limit { get; set; } = 20;
    }
}
