using ComicAPI.Enums;
using ComicAPI.Models;
using ComicApp.Models;
using Microsoft.EntityFrameworkCore;
namespace ComicApp.Data
{
    public static class DbContextExtension
    {
        public static IQueryable<Comic> GetTopDailyComics(this IQueryable<Comic> comics, IQueryable<DailyComicView> dailyComicViews)
        {
            DateTime date = DateTime.UtcNow;
            return (from comic in comics
                    join dailyView in dailyComicViews
                    on comic.ID equals dailyView.ComicID
                    where dailyView.ViewDate == date.Date
                    orderby dailyView.ViewCount descending
                    select comic);
        }
        public static IQueryable<Comic> GetTopWeeklyComics(this IQueryable<Comic> comics, IQueryable<DailyComicView> dailyComicViews)
        {
            DateTime date = DateTime.UtcNow;
            var startDate = date.Date.AddDays(-(int)date.DayOfWeek);
            var endDate = startDate.AddDays(7);

            var query = from comic in comics
                        join dailyView in dailyComicViews
                        on comic.ID equals dailyView.ComicID
                        where dailyView.ViewDate >= startDate && dailyView.ViewDate < endDate
                        group dailyView by comic into g
                        orderby g.Sum(x => x.ViewCount) descending
                        select g.Key;

            return query;
        }
        public static IQueryable<Comic> GetTopMonthlyComics(this IQueryable<Comic> comics, IQueryable<DailyComicView> dailyComicViews)
        {
            DateTime date = DateTime.UtcNow;
            var startDate = new DateTime(date.Year, date.Month, 1);
            var endDate = startDate.AddMonths(1);

            var query = from comic in comics
                        join dailyView in dailyComicViews
                        on comic.ID equals dailyView.ComicID
                        where dailyView.ViewDate >= startDate && dailyView.ViewDate < endDate
                        group dailyView by comic into g
                        orderby g.Sum(x => x.ViewCount) descending
                        select g.Key;

            return query;
        }
        // This method works with IQueryable<Comic> and applies ordering based on SortType
        public static IQueryable<Comic> OrderComicByType(this IQueryable<Comic> query, SortType sortType, IQueryable<DailyComicView>? dailyComicViews = null)
        {
            var date = DateTime.UtcNow;
            switch (sortType)
            {
                case SortType.Chapter:
                    return query.OrderByDescending(x => x.numchapter);
                case SortType.LastUpdate:
                    return query.OrderByDescending(x => x.UpdateAt);
                case SortType.NewComic:
                    return query.OrderByDescending(x => x.CreateAt).ThenBy(x => x.numchapter);
                case SortType.TopFollow:
                    // Implement logic for TopFollow if needed
                    break;
                case SortType.TopComment:
                    // Implement logic for TopComment if needed
                    break;
                case SortType.TopDay:
                    if (dailyComicViews != null)
                    {
                        return query.Join(
                            dailyComicViews.Where(d => d.ViewDate == date.Date),
                            comic => comic.ID,
                            dailyView => dailyView.ComicID,
                            (comic, dailyView) => new { comic, dailyView.ViewCount })
                        .OrderByDescending(x => x.ViewCount)
                        .Select(x => x.comic);
                    }
                    break;
                case SortType.TopWeek:
                    if (dailyComicViews != null)
                    {
                        var startDate = date.Date.AddDays(-(int)date.DayOfWeek);
                        var endDate = startDate.AddDays(7);
                        return query.Join(
                            dailyComicViews.Where(d => d.ViewDate >= startDate && d.ViewDate < endDate),
                            comic => comic.ID,
                            dailyView => dailyView.ComicID,
                            (comic, dailyView) => new { comic, dailyView.ViewCount })
                        .GroupBy(x => x.comic)
                        .OrderByDescending(g => g.Sum(y => y.ViewCount))
                        .Select(g => g.Key)
                        .AsQueryable();
                    }
                    break;
                case SortType.TopMonth:
                    if (dailyComicViews != null)
                    {
                        var startDate = new DateTime(date.Year, date.Month, 1);
                        var endDate = startDate.AddMonths(1);
                        return query.Join(
                            dailyComicViews.Where(d => d.ViewDate >= startDate && d.ViewDate < endDate),
                            comic => comic.ID,
                            dailyView => dailyView.ComicID,
                            (comic, dailyView) => new { comic, dailyView.ViewCount })
                        .GroupBy(x => x.comic)
                        .OrderByDescending(g => g.Sum(y => y.ViewCount))
                        .Select(g => g.Key)
                        .AsQueryable();
                    }
                    break;
                case SortType.TopAll:
                    return query.OrderByDescending(x => x.ViewCount);
                default:
                    return query.OrderBy(x => x.CreateAt);
            }
            return query;
        }
    }
}
