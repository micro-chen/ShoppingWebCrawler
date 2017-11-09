using System.Collections.Generic;
using System.Linq.Expressions;
namespace System.Linq
{

    //示范：public IQueryable<Person> Query(IQueryable<Person> source, string name, string code, string address)
    //{
    //    return source
    //        .WhereIf(p => p.Name.Contains(name), string.IsNullOrEmpty(name) == false)
    //        .WhereIf(p => p.Code.Contains(code), string.IsNullOrEmpty(code) == false)
    //        .WhereIf(p => p.Code.Contains(address), string.IsNullOrEmpty(address) == false);
    //}
    public static class WhereIfExtension
    {
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> source, Expression<Func<T, bool>> predicate, bool condition)
        {
            return condition ? source.Where(predicate) : source;
        }
        public static IQueryable<T> WhereIf<T>(this IQueryable<T> source, Expression<Func<T, int, bool>> predicate, bool condition)
        {
            return condition ? source.Where(predicate) : source;
        }
        public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> source, Func<T, bool> predicate, bool condition)
        {
            return condition ? source.Where(predicate) : source;
        }
        public static IEnumerable<T> WhereIf<T>(this IEnumerable<T> source, Func<T, int, bool> predicate, bool condition)
        {
            return condition ? source.Where(predicate) : source;
        }
    }
}