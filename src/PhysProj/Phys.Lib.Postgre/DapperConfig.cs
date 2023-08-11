using Phys.Lib.Postgres.Types;

namespace Phys.Lib.Postgres
{
    internal static class DapperConfig
    {
        public static void Configure()
        {
            Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;
            Dapper.SqlMapper.AddTypeHandler(new ListHandler<string>());
        }
    }
}
