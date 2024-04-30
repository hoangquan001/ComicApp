using ComicAPI.Enums;
using ComicApp.Models;
namespace ComicApp.Data
{
    public static class DbContextExtension
    {
        public static IQueryable<Comic> OrderComicByType(this IQueryable<Comic> query, SortType sortType)
        {
            switch (sortType)
            {
                case SortType.Chapter:
                    return query.OrderByDescending(x => x.Chapters.Count);
                case SortType.LastUpdate:
                    return query.OrderByDescending(x => x.UpdateAt);
                case SortType.NewComic:
                    return query.OrderByDescending(x => x.CreateAt).ThenBy(x => x.Chapters.Count);
                case SortType.TopFollow:
                case SortType.TopComment:
                case SortType.TopDay:
                case SortType.TopWeek:
                case SortType.TopMonth:
                case SortType.TopAll:
                    return query.OrderByDescending(x => x.ViewCount);

            }
            return query.OrderBy(x => x.CreateAt);
        }
    }
}