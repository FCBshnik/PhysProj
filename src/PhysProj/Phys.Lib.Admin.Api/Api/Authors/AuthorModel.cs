namespace Phys.Lib.Admin.Api.Api.Authors
{
    public class AuthorModel
    {
        public required string Code { get; set; }

        public string? Born { get; set; }

        public string? Died { get; set; }

        public List<AuthorInfoModel> Infos { get; set; } = new List<AuthorInfoModel>();

        public class AuthorInfoModel
        {
            public required string Language { get; set; }

            public string? FullName { get; set; }

            public string? Description { get; set; }
        }
    }
}
