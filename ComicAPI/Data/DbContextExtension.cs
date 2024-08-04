using ComicAPI.Enums;
using ComicAPI.Models;
using ComicApp.Models;
using Microsoft.EntityFrameworkCore;
namespace ComicApp.Data
{
    public static class DbContextExtension
    {

        // This method works with IQueryable<Comic> and applies ordering based on SortType
        public static IQueryable<Comic> GetComicsWithOrderByType(this ComicDbContext context, SortType sortType)
        {
            switch (sortType)
            {
                case SortType.Chapter:
                    return context.Comics.OrderByDescending(x => x.numchapter);
                case SortType.LastUpdate:
                    return context.Comics.OrderByDescending(x => x.UpdateAt);
                case SortType.NewComic:
                    return context.Comics.OrderByDescending(x => x.CreateAt).ThenBy(x => x.numchapter);
                case SortType.TopFollow:
                    // Implement logic for TopFollow if needed
                    break;
                case SortType.TopComment:
                    // Implement logic for TopComment if needed
                    break;
                case SortType.TopDay:
                    return context.GetTopDailyComics();
                case SortType.TopWeek:
                    return context.GetTopWeeklyComics();
                case SortType.TopMonth:
                    return context.GetTopMonthlyComics();
                case SortType.TopAll:
                    return context.Comics.OrderByDescending(x => x.ViewCount);
            }
            return context.Comics.OrderBy(x => x.CreateAt);

        }

    }
}
