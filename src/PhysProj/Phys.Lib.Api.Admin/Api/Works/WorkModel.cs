namespace Phys.Lib.Api.Admin.Api.Works
{
    public class WorkModel
    {
        public string Id { get; set; }

        public string Code { get; set; }

        public string Date { get; set; }

        public string Language { get; set; }

        public List<WorkInfoModel> Infos { get; set; } = new List<WorkInfoModel>();

        public List<string> AuthorsIds { get; set; } = new List<string>();

        public List<string> OriginalsIds { get; set; } = new List<string>();

        public class WorkInfoModel
        {
            public string Language { get; set; }

            public string Name { get; set; }

            public string Description { get; set; }
        }
    }
}
