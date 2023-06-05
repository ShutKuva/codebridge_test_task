using Core;

namespace BLL
{
    public static class PageHandler
    {
        public static IQueryable<T> Page<T>(this IQueryable<T> source, Page page)
        {
            int skip = page.PageNumber * page.NumberOfEntitiesOnPage;

            return source.Skip(skip > 0 ? skip : 0).Take(page.NumberOfEntitiesOnPage);
        }
    }
}