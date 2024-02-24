namespace Phys.Lib.Core.Stats
{
    public class WorksStatModel
    {
        public StatModel Total { get; set; } = new StatModel();

        public Dictionary<string, StatModel> PerLanguage { get; set; } = new Dictionary<string, StatModel>();

        public HashSet<string> Unreachable { get; set; } = new HashSet<string>();

        public HashSet<string> NoLang { get; set; } = new HashSet<string>();

        public HashSet<string> NoInfo { get; set; } = new HashSet<string>();

        public HashSet<string> NoPublic { get; set; } = new HashSet<string>();

        public class StatModel
        {
            public int Count { get; set; }

            public int CountWithFiles { get; set; }
        }
    }
}
