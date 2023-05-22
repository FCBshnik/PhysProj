namespace Phys.Lib.Core.Utils
{
    public static class CollectionUtils
    {
        private static readonly Dictionary<Type, object> emptyLists = new Dictionary<Type, object>();

        public static List<T> Empty<T>() 
        {
            var type = typeof(List<T>);
            if (emptyLists.ContainsKey(type))
                return (List<T>)emptyLists[type];
            var list = new List<T>();
            emptyLists[type] = list;
            return list;
        }
    }
}
