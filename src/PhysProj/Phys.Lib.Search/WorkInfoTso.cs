namespace Phys.Lib.Search
{
    public class WorkInfoTso
    {
        public required string Code { get; set; }

        public bool HasFiles { get; set; }

        public WorkInfoTso? Original { get; set; }

        public List<WorkInfoTso> SubWorks { get; set; } = new List<WorkInfoTso>();

        public List<WorkInfoTso> Translations { get; set; } = new List<WorkInfoTso>();

        public List<WorkInfoTso> Collected { get; set; } = new List<WorkInfoTso>();
    }
}
