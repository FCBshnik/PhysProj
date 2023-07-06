namespace Phys.Lib.Api.Admin.Api.Works
{
    public class WorkModel
    {
        public string Id { get; set; }

        public string Code { get; set; }

        public string Publish { get; set; }

        public string Language { get; set; }

        public List<WorkInfoModel> Infos { get; set; } = new List<WorkInfoModel>();

        public List<string> AuthorsCodes { get; set; } = new List<string>();

        public List<string> WorksCodes { get; set; } = new List<string>();

        public string OriginalCode { get; set; }

        public class WorkInfoModel
        {
            public string Language { get; set; }

            public string Name { get; set; }

            public string Description { get; set; }
        }
    }
}
