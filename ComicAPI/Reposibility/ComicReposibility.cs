
using System.Collections.Concurrent;
using ComicAPI.Enums;
using ComicAPI.Services;
using ComicApp.Data;
using ComicApp.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

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
        .Select(x => new ComicDTO(x)
        {
            genres = x.Genres.Select(g => new GenreLiteDTO(g)),
            CoverImage = _urlService.GetComicCoverImagePath(x.CoverImage),
            Chapters = x.Chapters.Where(c => c.ID == x.lastchapter).Select(ch => new ChapterDTO(ch))
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

    private async Task<int> GetComicTotalPageAsync(int genre = -1, ComicStatus status = ComicStatus.All)
    {

        string key = genre.ToString() + status.ToString();
        if (!_memoryCache.TryGetValue("TotalPageComicKey", out ConcurrentDictionary<string, int>? cachedData))
        {
            cachedData = new ConcurrentDictionary<string, int>();
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                 .SetSlidingExpiration(TimeSpan.FromHours(1)); // Example: cache for 10 minutes
            _memoryCache.Set("TotalPageComicKey", cachedData, cacheEntryOptions);
        }

        if (!cachedData!.TryGetValue(key, out int totalcomic))
        {
            totalcomic = await _dbContext.Comics.Where(x => (status == ComicStatus.All || x.Status == (int)status) && (genre == -1 || x.Genres.Any(g => genre == g.ID))).CountAsync();
            cachedData.TryAdd(key, totalcomic);
        }
        return totalcomic;

    }
    public Comic AddComic(Comic comic)
    {
        throw new NotImplementedException();
    }

    public async Task<ListComicDTO?> GetHotComics(int page, int step)
    {
        var query = _dbContext.Comics
        .OrderByDescending(keySelector: x => x.UpdateAt)
        .Where(x => x.Status == (int)ComicStatus.Ongoing)
        .Where(x => x.UpdateAt >= DateTime.UtcNow.AddMonths(-6))
        .AsQueryable();
        var data = await query
        .Skip((page - 1) * step)
        .Take(step)
        .Select(x => new ComicDTO(x)
        {
            genres = x.Genres.Select(g => new GenreLiteDTO(g)),
            CoverImage = _urlService.GetComicCoverImagePath(x.CoverImage),
            Chapters = x.Chapters.Where(c => c.ID == x.lastchapter).Select(ch => new ChapterDTO(ch))
        }).ToListAsync();

        if (data != null)
        {
            int totalcomic = await query.CountAsync();
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

    public async Task<ListComicDTO?> GetComics(int page, int step, int genre = -1, ComicStatus status = ComicStatus.All,
     SortType sort = SortType.TopAll)
    {
        var queryBuilder = _dbContext.GetComicsWithOrderByType(sort)
        .Where(x => (status == ComicStatus.All || x.Status == (int)status) && (genre == -1 || x.Genres.Any(g => genre == g.ID)));
        var data = await queryBuilder
        .Skip((page - 1) * step)
        .Take(step)
        .Select(x => new ComicDTO(x)
        {
            genres = x.Genres.Select(g => new GenreLiteDTO(g)),
            CoverImage = _urlService.GetComicCoverImagePath(x.CoverImage),
            Chapters = x.Chapters.Where(c => c.ID == x.lastchapter).Select(ch => new ChapterDTO(ch))
        })
        .ToListAsync();
        if (data != null)
        {
            int totalcomic = await GetComicTotalPageAsync(genre, status);

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
        .Select(x => new ComicDTO(x)
        {
            genres = x.Genres.Select(g => new GenreLiteDTO(g)),
            CoverImage = _urlService.GetComicCoverImagePath(x.CoverImage),
            Chapters = x.Chapters.Where(c => c.ID == x.lastchapter).Select(ch => new ChapterDTO(ch))
        })
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
        var chapter = await _dbContext.Chapters
        .Where(x => x.ID == chapter_id)
        .AsNoTracking()
        .FirstOrDefaultAsync();

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

    public async Task<List<ChapterDTO>?> GetChapters(int comicid)
    {

        var chapters = await _dbContext.Chapters
        .AsNoTracking()
        .Where(x => x.ComicID == comicid)
        .OrderByDescending(x => Convert.ToDouble(x.Url))
        .Select(x => new ChapterDTO(x))
        .ToListAsync();
        return chapters;

    }



    public async Task<List<ComicDTO>> GetComicByKeyword(string keyword)
    {
        return GetComicByKeyword(await GetAllComics(), keyword);
    }
    private List<ComicDTO> GetComicByKeyword(List<ComicDTO> data, string keyword)
    {
        keyword = keyword.Replace("-", " ");
        keyword = SlugHelper.CreateSlug(keyword);
        var vec1 = new Dictionary<string, int>();
        SlugHelper.GetTermFrequencyVector(keyword, ref vec1);
        List<(ComicDTO comic, int similarity)> result = new List<(ComicDTO, int)>();

        for (int i = 0; i < data.Count; i++)
        {
            Dictionary<string, int> vec2 = new Dictionary<string, int>();
            SlugHelper.GetTermFrequencyVector(data[i].Url, ref vec2);
            if (!string.IsNullOrEmpty(data[i].OtherName))
            {
                var otherName = SlugHelper.CreateSlug(data[i].OtherName);
                SlugHelper.GetTermFrequencyVector(otherName, ref vec2);
            }

            double dotProduct = 0;
            double norm1 = 0;
            double norm2 = 0;

            foreach (string key in vec1.Keys)
            {
                if (vec2.ContainsKey(key))
                    dotProduct += vec1[key] * vec2[key];
                norm1 += vec1[key] * vec1[key];
            }
            norm2 = vec2.Keys.Sum(x => vec2[x] * vec2[x]);

            var a = dotProduct / (Math.Sqrt(norm1) * Math.Sqrt(norm2));
            if (a > 0.1)
            {
                result.Add((data[i], (int)(a * 100)));
            }
        }

        return result.OrderByDescending(x => x.similarity).Take(5).Select(x => x.comic).ToList();

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
            keyword = keyword.Replace("-", " ");
            comicsQuery = comicsQuery.Select(x => new
            {
                comic = x,
                similarity = EF.Functions.TrigramsSimilarity(x.Title, keyword)
            })
            .Where(x => x.similarity > 0.1)
            .OrderByDescending(x => x.similarity)
            .Select(x => x.comic)
            ;
        }

        var data = await comicsQuery
        .Skip((page - 1) * step)
        .Take(step)
        .Select(x => new ComicDTO(x)
        {
            genres = x.Genres.Select(g => new GenreLiteDTO(g)),

            CoverImage = _urlService.GetComicCoverImagePath(x.CoverImage),
            Chapters = x.Chapters.Where(c => c.ID == x.lastchapter).Select(ch => new ChapterDTO(ch))
        })
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
        .Select(x => new ComicDTO(x)
        {
            genres = x.Genres.Select(g => new GenreLiteDTO(g)),

            CoverImage = _urlService.GetComicCoverImagePath(x.CoverImage),
            Chapters = x.Chapters.Where(c => c.ID == x.lastchapter).Select(ch => new ChapterDTO(ch))
        })
        .AsNoTracking()
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
        .Select(x => new ComicDTO(x)
        {
            genres = x.Genres.Select(g => new GenreLiteDTO(g)),

            CoverImage = _urlService.GetComicCoverImagePath(x.CoverImage),
            Chapters = x.Chapters.Where(c => c.ID == x.lastchapter).Select(ch => new ChapterDTO(ch))
        }).ToListAsync();
        var weeklyComics = await _dbContext.GetTopDailyComics(TopViewType.Week)
        .Take(step)
        .Select(x => new ComicDTO(x)
        {
            genres = x.Genres.Select(g => new GenreLiteDTO(g)),

            CoverImage = _urlService.GetComicCoverImagePath(x.CoverImage),
            Chapters = x.Chapters.Where(c => c.ID == x.lastchapter).Select(ch => new ChapterDTO(ch))
        })
        .ToListAsync();
        var monthlyComics = await _dbContext.GetTopDailyComics(TopViewType.Month)
        .Take(step)
        .Select(x => new ComicDTO(x)
        {
            genres = x.Genres.Select(g => new GenreLiteDTO(g)),

            CoverImage = _urlService.GetComicCoverImagePath(x.CoverImage),
            Chapters = x.Chapters.Where(c => c.ID == x.lastchapter).Select(ch => new ChapterDTO(ch))
        })
        .ToListAsync();

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
        .Select(x => new ComicDTO(x)
        {
            CoverImage = _urlService.GetComicCoverImagePath(x.CoverImage),
            genres = x.Genres.Select(g => new GenreLiteDTO(g)),
            Chapters = x.Chapters.Where(c => c.ID == x.lastchapter).Select(ch => new ChapterDTO(ch))
        }).ToListAsync();
        return comics;
    }

}