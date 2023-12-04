namespace Phys.Lib.Core.Stats
{
    public class WorksStatModel
    {
        public StatModel Total { get; set; } = new StatModel();

        public Dictionary<string, StatModel> PerLanguage { get; set; } = new Dictionary<string, StatModel>();

        public List<string> Unreachable { get; set; } = new List<string>();

        public class StatModel
        {
            public int Count { get; set; }

            public int CountWithFiles { get; set; }
        }
    }
}
