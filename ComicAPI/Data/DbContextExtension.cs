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
                    return query.OrderByDescending(keySelector: x => x.Chapters.Max(c => c.UpdateAt));
                // case SortType.TopFollow:
                //     return query.OrderBy(x => x.Follow);
                // case SortType.TopComment:
                //     return query.OrderBy(x => x.Comment);
                case SortType.NewComic:
                    return query.OrderByDescending(x => x.CreateAt).ThenBy(x => x.Chapters.Count);
                // case SortType.TopDay:
                //     return query.OrderBy(x => x.CreateAt);
                // case SortType.TopWeek:
                //     return query.OrderBy(x => x.CreateAt);
                // case SortType.TopMonth:
                // query.SelectMany(x => x.Chapters).Where(c => c.UpdateAt > DateTime.Now.AddDays(-30));
                // return query.Where(c =>query.Contains(c)).OrderBy(x => x.Chapters.Sum(x=>x.ViewCount));
                case SortType.TopAll:
                    return query.OrderByDescending(x => x.Chapters.Sum(c => c.ViewCount));

            }
            return query.OrderBy(x => x.CreateAt);
        }
    }
}