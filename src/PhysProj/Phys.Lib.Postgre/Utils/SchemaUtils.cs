using SqlKata;
using System.Linq.Expressions;
using System.Reflection;

namespace Phys.Lib.Postgres.Utils
{
    internal static class SchemaUtils
    {
        internal static string GetColumn<T, P>(Expression<Func<T, P>> property)
        {
            var memberExpr = (MemberExpression)property.Body;
            var attr = memberExpr.Member.GetCustomAttribute<ColumnAttribute>();
            return attr?.Name ?? throw new ApplicationException($"missed {nameof(ColumnAttribute)} on member {memberExpr.Member.Name} of {memberExpr.Type.Name}");
        }
    }
}
