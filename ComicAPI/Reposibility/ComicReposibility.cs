using System.Collections.Concurrent;
using System.Globalization;
using System.Text;
using ComicAPI.Enums;
using ComicAPI.Models;
using ComicAPI.Services;
using ComicApp.Data;
using ComicApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.VisualBasic;

public class ComicReposibility : IComicReposibility
{
    private readonly ComicDbContext _dbContext;
    private readonly UrlService _urlService;
    private readonly IMemoryCache _memoryCache;

    public ComicReposibility(ComicDbContext dbContext, IMemoryCache cache, UrlService urlService)
    {
        _memoryCache = cache;
        _dbContext = dbContext;
        _urlService = urlService;

    }
    private List<string> Nogenres = new List<string> { "gender-bender","adult" ,"dam-my",
                                        "gender-bender","shoujo-ai","shounen-ai",
                                        "smut","soft-yaoi","soft-yuri"};
    private async Task<List<ComicDTO>> _getAllComicsFromDB()
    {
        return await _dbContext.Comics
        .OrderBy(x => x.ID)
        .Select(x => new ComicDTO
        {
            ID = x.ID,
            Title = x.Title,
            OtherName = x.OtherName,
            Author = x.Author,
            Url = x.Url,
            CoverImage = _urlService.GetComicCoverImagePath(x.CoverImage),
            Description = x.Description,
            Status = x.Status,
            Rating = x.Rating,
            ViewCount = x.ViewCount,
            UpdateAt = x.UpdateAt,
            genres = x.Genres.Select(g => new GenreLiteDTO { ID = g.ID, Title = g.Title }),
            Chapters = x.Chapters.Where(c => c.ID == x.lastchapter).Select(ch => ChapterSelector(ch)).ToList()
        })
        .ToListAsync();
    }

    public async Task<List<ComicDTO>> GetAllComics()
    {
        const string cacheKey = "ALL_COMIC_KEY";

        if (!_memoryCache.TryGetValue(cacheKey, out List<ComicDTO>? cachedData))
        {
            cachedData = await _getAllComicsFromDB();
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                 .SetSlidingExpiration(TimeSpan.FromMinutes(10)); // Example: cache for 10 minutes
            _memoryCache.Set(cacheKey, cachedData, cacheEntryOptions);
        }

        return cachedData!;
    }

    // private async Task<int> GetComicTotalPageAsync(int genre = -1, ComicStatus status = ComicStatus.All)
    // {

    //     string key = genre.ToString() + status.ToString();
    //     if (!_memoryCache.TryGetValue("TotalPageComicKey", out ConcurrentDictionary<string, int>? cachedData))
    //     {
    //         cachedData = new ConcurrentDictionary<string, int>();
    //         var cacheEntryOptions = new MemoryCacheEntryOptions()
    //              .SetSlidingExpiration(TimeSpan.FromHours(1)); // Example: cache for 10 minutes
    //         _memoryCache.Set("TotalPageComicKey", cachedData, cacheEntryOptions);
    //     }


    //     if (!cachedData!.TryGetValue(key, out int totalcomic))
    //     {
    //         totalcomic = await _dbContext.Comics.Where(x => (status == ComicStatus.All || x.Status == (int)status) && (genre == -1 || x.Genres.Any(g => genre == g.ID))).CountAsync();
    //         cachedData.TryAdd(key, totalcomic);
    //     }
    //     return totalcomic;

    // }
    public Comic AddComic(Comic comic)
    {
        throw new NotImplementedException();
    }

    public async Task<ListComicDTO?> GetComics(int page, int step, int hot, int genre = -1, ComicStatus status = ComicStatus.All,
     SortType sort = SortType.TopAll)
    {
        if (hot == 1)
        {
            status = ComicStatus.Ongoing;
            sort = SortType.LastUpdate;
        }
        var queryBuilder = _dbContext.GetComicsWithOrderByType(sort)
        .Where(x => (status == ComicStatus.All || x.Status == (int)status) && (genre == -1 || x.Genres.Any(g => genre == g.ID)));
        if (hot == 1)
        {
            queryBuilder = queryBuilder.Where(x => x.UpdateAt >= DateTime.Now.AddDays(-7 * 6));
        }

        var data = await queryBuilder.Select(x => new ComicDTO
        {
            ID = x.ID,
            Title = x.Title,
            OtherName = x.OtherName,
            Author = x.Author,
            Url = x.Url,
            CoverImage = _urlService.GetComicCoverImagePath(x.CoverImage),
            Description = x.Description,
            Status = x.Status,
            Rating = x.Rating,
            ViewCount = x.ViewCount,
            UpdateAt = x.UpdateAt,
            genres = x.Genres.Select(g => new GenreLiteDTO { ID = g.ID, Title = g.Title }),
            Chapters = x.Chapters.Where(c => c.ID == x.lastchapter).Select(ch => ChapterSelector(ch)).ToList()
        })
        .Skip((page - 1) * step)
        .Take(step)
        .ToListAsync();
        if (data != null)
        {
            int totalcomic = queryBuilder.Count();
            ListComicDTO list = new ListComicDTO
            {
                totalpage = (int)MathF.Ceiling((float)totalcomic / step),
                Page = page,
                Step = step,
                comics = data
            };
            return list;
        }
        return null;
    }

    private static ChapterDTO ChapterSelector(Chapter x)
    {
        return new ChapterDTO
        {
            ID = x.ID,
            Title = x.Title,
            ViewCount = x.ViewCount,
            UpdateAt = x.UpdateAt,
            Slug = x.Url
        };
    }

    public List<Genre> GetGenres()
    {
        throw new NotImplementedException();
    }

    public byte[] LoadImage(string url)
    {
        throw new NotImplementedException();
    }
    private async Task<ComicDTO?> _getComicFromDB(string key)
    {
        bool isID = int.TryParse(key, out int id2);
        var data = await _dbContext.Comics
        .Where(x => isID ? x.ID == id2 : x.Url == key)
        .Select(x => new ComicDTO
        {
            ID = x.ID,
            Title = x.Title,
            OtherName = x.OtherName,
            Author = x.Author,
            Url = x.Url,
            CoverImage = _urlService.GetComicCoverImagePath(x.CoverImage),
            Description = x.Description,
            Status = x.Status,
            Rating = x.Rating,
            UpdateAt = x.UpdateAt,
            ViewCount = x.ViewCount,
            genres = x.Genres.Select(g => new GenreLiteDTO { ID = g.ID, Title = g.Title }).ToList(),
            Chapters = x.Chapters.OrderByDescending(x => Convert.ToDouble(x.Url)).Select(x => ChapterSelector(x)).ToList()
        })
        .AsSplitQuery()
        .FirstOrDefaultAsync();

        return data;

    }

    public async Task<ComicDTO?> GetComic(string key)
    {
        string keysave = string.Format("comic-{0}", key);
        if (!_memoryCache.TryGetValue(keysave, out ComicDTO? cachedData))
        {
            cachedData = await _getComicFromDB(key); ;
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                 .SetSlidingExpiration(TimeSpan.FromMinutes(5)); // Example: cache for 10 minutes
            _memoryCache.Set(keysave, cachedData, cacheEntryOptions);
        }
        return cachedData;

    }

    public async Task<Chapter?> GetChapter(int chapter_id)
    {
        var chapter = await _dbContext.Chapters.Where(x => x.ID == chapter_id).FirstOrDefaultAsync();
        if (chapter == null) return null;
        // string cacheKey = string.Format("chapter-{0}", chapter_id);
        // if (!_memoryCache.TryGetValue(cacheKey, out List<ComicDTO>? cachedData))
        // {
        //     cachedData = await _getComicRecommendFromDB();
        //     var cacheEntryOptions = new MemoryCacheEntryOptions()
        //          .SetSlidingExpiration(TimeSpan.FromMinutes(5)); // Reset each 5 minutes
        //     _memoryCache.Set(cacheKey, cachedData, cacheEntryOptions);
        // }
        return chapter;
    }

    public async Task<List<ChapterDTO>?> GetChaptersComic(string key)
    {

        bool isID = int.TryParse(key, out int id2);
        var data = await _dbContext.Comics.Where(x => isID ? x.ID == id2 : x.Url == key)
        .Select(x => x.Chapters.Select(x => ChapterSelector(x)).ToList())
        .FirstOrDefaultAsync();
        return data;

    }

    public async Task<List<ComicDTO>?> SearchComicsByKeyword(string keyword)
    {
        keyword = keyword.ToLower();



        // var data = await _dbContext.Comics
        // // .Where(
        // //     x =>
        // //     EF.Functions.ILike(x.Title, $"%{keyword}%") ||
        // //     EF.Functions.ILike(x.Url, $"%{keyword}%") ||
        // //     EF.Functions.ILike(x.OtherName, $"%{keyword}%")
        // // )
        // .Select(x => new ComicDTO
        // {
        //     ID = x.ID,
        //     Title = x.Title,
        //     Author = x.Author,
        //     Url = x.Url,
        //     CoverImage = x.CoverImage,
        //     Description = x.Description,
        //     Status = x.Status,
        //     Rating = x.Rating,
        //     UpdateAt = x.UpdateAt,
        //     ViewCount = x.ViewCount
        // })
        // .ToListAsync();

        List<ComicDTO> result = new List<ComicDTO>();
        await Task.Delay(1000);

        return result;
    }


    public static string RemoveDiacritics(string text)
    {
        while (text.IndexOf("  ") != -1) // kiểm tra xem có dấu 2 dấu cách nào liền nhau hay không
        {
            // thực hiện câu lệnh trong vòng lặp này chứng tỏ là có 2 dấu cách liền nhau
            text = text.Remove(text.IndexOf("  "), 1); // loại bỏ đi 1 trong 2 dấu cách
        }
        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
    }

    public async Task<ListComicDTO?> GetComicBySearchAdvance(SortType sort = SortType.TopAll, ComicStatus status = ComicStatus.All,
     List<int>? genres = null, int page = 1, int step = 100, List<int>? Nogenres = null, int? year = null, string? keyword = null)
    {


        genres ??= new List<int> { -1 };
        Nogenres ??= new List<int> { -1 };

        var comicsQuery = _dbContext.GetComicsWithOrderByType(sort);

        if (status != ComicStatus.All)
        {
            comicsQuery = comicsQuery.Where(x => x.Status == (int)status);
        }
        if (year > 0)
        {
            comicsQuery = comicsQuery.Where(x => (int)x.UpdateAt.Year == (int)year);
        }
        // Áp dụng bộ lọc thể loại (genres)
        if (!genres.Contains(-1))
        {
            var genreIds = genres.ToHashSet();
            comicsQuery = comicsQuery.Where(x => x.Genres.Select(g => g.ID).Intersect(genreIds).Count() == genreIds.Count);
        }

        // Áp dụng bộ lọc loại trừ thể loại (Nogenres)
        if (!Nogenres.Contains(-1))
        {
            var nogenreIds = Nogenres.ToHashSet();
            comicsQuery = comicsQuery.Where(x => !x.Genres.Select(g => g.ID).Any(gId => nogenreIds.Contains(gId)));
        }
        if (!string.IsNullOrEmpty(keyword))
        {
            keyword = RemoveDiacritics(keyword).ToLower();
            comicsQuery = comicsQuery.Where(comic =>
                comic.Url.ToLower().Contains(RemoveDiacritics(keyword).Trim().ToLower().Replace(" ", "-"))
                );
        }


        // Execute query and get data

        var data = await comicsQuery

            .Select(x => new ComicDTO
            {
                ID = x.ID,
                Title = x.Title,
                OtherName = x.OtherName,
                Author = x.Author,
                Url = x.Url,
                CoverImage = _urlService.GetComicCoverImagePath(x.CoverImage),
                Description = x.Description,
                Status = x.Status,
                Rating = x.Rating,
                ViewCount = x.ViewCount,
                UpdateAt = x.UpdateAt,
                genres = x.Genres.Select(g => new GenreLiteDTO { ID = g.ID, Title = g.Title }),
                Chapters = x.Chapters.Where(c => c.ID == x.lastchapter).Select(ch => ChapterSelector(ch)).ToList()
            })
            .Skip((page - 1) * step)
            .Take(step)
            .ToListAsync();

        // Get total count

        // Get total count
        var totalComics = await comicsQuery.CountAsync();

        if (data != null && data.Any())
        {
            ListComicDTO list = new ListComicDTO
            {
                totalpage = (int)MathF.Ceiling((float)totalComics / step),
                Page = page,
                Step = step,
                comics = data
            };

            return list;
        }

        return null;
    }
    private async Task<List<ComicDTO>?> _getComicRecommendFromDB()
    {
        var comicsQuery = _dbContext.Comics.AsQueryable();

        // Áp dụng bộ lọc loại trừ thể loại (Nogenres)

        var nogenreIds = Nogenres.ToHashSet();

        var datenow = DateTime.UtcNow;
        comicsQuery = comicsQuery.Where(x => !x.Genres.Select(g => g.Slug).Any(gSlug => nogenreIds.Contains(gSlug)));
        comicsQuery = comicsQuery.Where(x => (datenow - x.UpdateAt).TotalDays <= 700
         && x.ViewCount >= 1000000);
        // query number chaper>10
        comicsQuery = comicsQuery.Where(x => x.numchapter > 10);
        // Execute query and get data
        var data = await comicsQuery
        .Select(x => new ComicDTO
        {
            ID = x.ID,
            Title = x.Title,
            OtherName = x.OtherName,
            Author = x.Author,
            Url = x.Url,
            CoverImage = _urlService.GetComicCoverImagePath(x.CoverImage),
            Description = x.Description,
            Status = x.Status,
            Rating = x.Rating,
            ViewCount = x.ViewCount,
            UpdateAt = x.UpdateAt,
            genres = x.Genres.Select(g => new GenreLiteDTO { ID = g.ID, Title = g.Title }),
            Chapters = x.Chapters.Where(c => c.ID == x.lastchapter).Select(ch => ChapterSelector(ch)).ToList()
        })
        .ToListAsync();
        List<float> data2 = data.Select(x => (float)x.ViewCount).ToList();
        data = ServiceUtilily.SampleList(data, data2, 30);

        if (data != null && data.Any())
        {
            return data;
        }
        return null;
    }

    public async Task<List<ComicDTO>?> GetComicRecommend()
    {

        const string cacheKey = "RECOMMEND_COMIC_KEY";
        if (!_memoryCache.TryGetValue(cacheKey, out List<ComicDTO>? cachedData))
        {
            cachedData = await _getComicRecommendFromDB();
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                 .SetSlidingExpiration(TimeSpan.FromMinutes(5)); // Reset each 5 minutes
            _memoryCache.Set(cacheKey, cachedData, cacheEntryOptions);
        }

        return cachedData!;
    }

    public async Task<ComicTopViewDTO?> _getTopViewComicsFromDB(int step)
    {
        var dailyComics = await _dbContext.GetTopDailyComics(TopViewType.Day)
        .Take(step)
        .Select(x => new ComicDTO
        {
            ID = x.ID,
            Title = x.Title,
            OtherName = x.OtherName,
            Author = x.Author,
            Url = x.Url,
            CoverImage = _urlService.GetComicCoverImagePath(x.CoverImage),
            Description = x.Description,
            Status = x.Status,
            Rating = x.Rating,
            ViewCount = x.ViewCount,
            UpdateAt = x.UpdateAt,
            genres = x.Genres.Select(g => new GenreLiteDTO { ID = g.ID, Title = g.Title }),
            Chapters = x.Chapters.Where(c => c.ID == x.lastchapter).Select(ch => ChapterSelector(ch)).ToList()
        }).ToListAsync();
        var weeklyComics = await _dbContext.GetTopDailyComics(TopViewType.Week)
        .Take(step)
        .Select(x => new ComicDTO
        {
            ID = x.ID,
            Title = x.Title,
            OtherName = x.OtherName,
            Author = x.Author,
            Url = x.Url,
            CoverImage = _urlService.GetComicCoverImagePath(x.CoverImage),
            Description = x.Description,
            Status = x.Status,
            Rating = x.Rating,
            ViewCount = x.ViewCount,
            UpdateAt = x.UpdateAt,
            genres = x.Genres.Select(g => new GenreLiteDTO { ID = g.ID, Title = g.Title }),
            Chapters = x.Chapters.Where(c => c.ID == x.lastchapter).Select(ch => ChapterSelector(ch)).ToList()
        }).ToListAsync();
        var monthlyComics = await _dbContext.GetTopDailyComics(TopViewType.Month)
        .Take(step)
        .Select(x => new ComicDTO
        {
            ID = x.ID,
            Title = x.Title,
            OtherName = x.OtherName,
            Author = x.Author,
            Url = x.Url,
            CoverImage = _urlService.GetComicCoverImagePath(x.CoverImage),
            Description = x.Description,
            Status = x.Status,
            Rating = x.Rating,
            ViewCount = x.ViewCount,
            UpdateAt = x.UpdateAt,
            genres = x.Genres.Select(g => new GenreLiteDTO { ID = g.ID, Title = g.Title }),
            Chapters = x.Chapters.Where(c => c.ID == x.lastchapter).Select(ch => ChapterSelector(ch)).ToList()
        }).ToListAsync();

        ComicTopViewDTO? data = new ComicTopViewDTO()
        {
            DailyComics = dailyComics,
            WeeklyComics = weeklyComics,
            MonthlyComics = monthlyComics
        };
        return data;
    }
    public async Task<ComicTopViewDTO?> GetTopViewComics(int step)
    {
        const string cacheKey = "TOPVIEW_COMIC_KEY";
        if (!_memoryCache.TryGetValue(cacheKey, out ComicTopViewDTO? cachedData))
        {
            cachedData = await _getTopViewComicsFromDB(step);
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                 .SetSlidingExpiration(TimeSpan.FromMinutes(10)); // Reset each 10 minutes
            _memoryCache.Set(cacheKey, cachedData, cacheEntryOptions);
        }
        return cachedData!;
    }

    public async Task UpdateViewComic(HashSet<int> comicview)
    {
        var comicIds = comicview.ToList();

        var comicsToUpdate = _dbContext.Comics
                                             .Where(c => comicIds.Contains(c.ID));


        var chapterViewCounts = _dbContext.Chapters
                                                .Where(c => comicIds.Contains(c.ComicID))
                                                .GroupBy(c => c.ComicID)
                                                .Select(g => new { ComicID = g.Key, TotalViewCount = g.Sum(c => c.ViewCount) });

        var chapterViewCountDict = chapterViewCounts.ToDictionary(x => x.ComicID, x => x.TotalViewCount);

        foreach (var comic in comicsToUpdate)
        {
            if (chapterViewCountDict.TryGetValue(comic.ID, out int totalViewCount))
            {
                comic.ViewCount = totalViewCount;
            }
            else
            {
                comic.ViewCount = 0;
            }
        }

        await _dbContext.SaveChangesAsync();
    }
    public async Task UpdateViewChapter(Dictionary<int, int> chapterviews)
    {

        var chapterIds = chapterviews.Keys.ToList();
        var chaptersToUpdate = _dbContext.Chapters
                                            .Where(c => chapterIds.Contains(c.ID));


        foreach (var chapter in chaptersToUpdate)
        {
            chapter.ViewCount += chapterviews[chapter.ID];
        }


        await _dbContext.SaveChangesAsync();
    }

    public async Task<List<ComicDTO>> GetComicsByIds(List<int> ids)
    {
        var comics = await _dbContext.Comics
        .Where(x => ids.Contains(x.ID))
        .Select(x => new ComicDTO
        {
            ID = x.ID,
            Title = x.Title,
            OtherName = x.OtherName,
            Author = x.Author,
            Url = x.Url,
            CoverImage = _urlService.GetComicCoverImagePath(x.CoverImage),
            Description = x.Description,
            Status = x.Status,
            Rating = x.Rating,
            ViewCount = x.ViewCount,
            UpdateAt = x.UpdateAt,
            genres = x.Genres.Select(g => new GenreLiteDTO { ID = g.ID, Title = g.Title }),
            // Chapters = x.Chapters.Where(c => c.ID == x.lastchapter).Select(ch => ChapterSelector(ch)).ToList()
        }).ToListAsync();
        return comics;
    }
}