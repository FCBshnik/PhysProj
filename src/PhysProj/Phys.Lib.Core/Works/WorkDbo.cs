namespace Phys.Lib.Core.Works
{
    public class WorkDbo
    {
        public string Id { get; set; }

        public string Code { get; set; }

        public string Date { get; set; }

        public string Language { get; set; }

        public List<InfoDbo> Infos { get; set; } = new List<InfoDbo>();

        public List<string> AuthorsIds { get; set; } = new List<string>();

        public List<string> EditionsIds { get; set; } = new List<string>();

        public class InfoDbo
        {
            public string Language { get; set; }

            public string Name { get; set; }

            public string Description { get; set; }
        }
    }
}
