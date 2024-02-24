namespace Phys.Lib.Core.Stats
{
    public interface IStatService
    {
        SystemStatsModel GetLibraryStats();

        SystemStatsModel CalcLibraryStats();
    }
}
