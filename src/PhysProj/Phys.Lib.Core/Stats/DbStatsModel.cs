namespace Phys.Lib.Core.Stats
{
    public class DbStatsModel
    {
        public required string Name { get; set; }

        public required LibraryStatsModel Library { get; set; }
    }
}
