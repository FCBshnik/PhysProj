using Dapper;
using System.Data;

namespace Phys.Lib.Postgres.Types
{
    internal class ListHandler<T> : SqlMapper.TypeHandler<List<T>>
    {
        public override List<T> Parse(object value)
        {
            T[] typedValue = (T[])value;
            return typedValue?.ToList() ?? new List<T>();
        }

        public override void SetValue(IDbDataParameter parameter, List<T> value)
        {
            parameter.Value = value;
        }
    }
}
