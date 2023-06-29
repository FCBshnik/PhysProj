using Phys.Lib.Core.Authors;

namespace Phys.Lib.Core.Works
{
    public class WorkUpdate
    {
        public string? Date { get; set; }

        public string? Language { get; set; }

        public WorkDbo.InfoDbo? AddInfo { get; set; }

        public string? DeleteInfo { get; set; }

        public AuthorDbo? AddAuthor { get; set; }

        public string? DeleteAuthor { get; set; }

        public WorkDbo? AddWork { get; set; }

        public string? DeleteWork { get; set; }

        public WorkDbo? Original { get; set; }
    }
}
