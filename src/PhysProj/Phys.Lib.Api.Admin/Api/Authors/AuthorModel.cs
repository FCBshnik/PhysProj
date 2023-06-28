namespace Phys.Lib.Api.Admin.Api.Authors
{
    public class AuthorModel
    {
        public string Id { get; set; }

        public string Code { get; set; }

        public string Born { get; set; }

        public string Died { get; set; }

        public List<AuthorInfoModel> Infos { get; set; } = new List<AuthorInfoModel>();

        public class AuthorInfoModel
        {
            public string Language { get; set; }

            public string Name { get; set; }

            public string Description { get; set; }
        }
    }
}
