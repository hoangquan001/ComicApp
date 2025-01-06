
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

    private readonly bool CacheEnabled = true;

    private List<string> NoGenres = new List<string> { "gender-bender","adult" ,"dam-my",
                                        "gender-bender","shoujo-ai","shounen-ai",
                                        "smut","soft-yaoi","soft-yuri"};
    public ComicReposibility(ComicDbContext dbContext, IMemoryCache cache, UrlService urlService)
    {
        _memoryCache = cache;
        _dbContext = dbContext;
        _urlService = urlService;

    }
    private async Task<Dictionary<int, Comic>> _getAllComicsFromDB()
    {
        var data = await _dbContext.Comics
        .Include(x => x.Genres)
        .AsNoTracking()
        .ToDictionaryAsync(x => x.ID, x => x);
        return data;
    }

    public async Task<Dictionary<int, Comic>> GetAllComics()
    {
        const string cacheKey = "ALL_COMIC_KEY";

        if (/*!CacheEnabled ||*/ !_memoryCache.TryGetValue(cacheKey, out Dictionary<int, Comic>? cachedData))
        {
            DateTime start = DateTime.Now;
            cachedData = await _getAllComicsFromDB();
            TimeSpan timeItTook = DateTime.Now - start;
            Console.WriteLine($"GetAllComics took {timeItTook.TotalMilliseconds} ms");
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
        .Include(x => x.Genres)
        .Select(x => new ComicDTO(x, _urlService)
        {
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
        .Include(x => x.Genres)
        .Select(x => new ComicDTO(x, _urlService)
        {
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
    private async Task<Comic?> _getComicFromDB(string key)
    {
        bool isID = int.TryParse(key, out int id2);

        var data = await _dbContext.Comics
        .Where(x => isID ? x.ID == id2 : x.Url == key)
        .Include(x => x.Genres)
        .Select(x => new
        {
            Comic = x,
            CoverImage = _urlService.GetComicCoverImagePath(x.CoverImage),
            Chapters = x.Chapters.Where(c => c.ID == x.lastchapter)
        })
        .FirstOrDefaultAsync();
        if (data == null) return null;

        Comic comic = data.Comic;
        comic.CoverImage = data.CoverImage;
        comic.Chapters = data.Chapters.ToList();
        return comic;

    }

    public async Task<Comic?> GetComic(string key)
    {
        string keysave = string.Format("comic-{0}", key);
        if (!CacheEnabled || !_memoryCache.TryGetValue(keysave, out Comic? cachedData))
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
        string cacheKey = string.Format("chapter-{0}", chapter_id);
        if (!CacheEnabled || !_memoryCache.TryGetValue(cacheKey, out Chapter? cachedData))
        {
            cachedData = await _dbContext.Chapters
            .Where(x => x.ID == chapter_id)
            .AsNoTracking()
            .FirstOrDefaultAsync();
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                 .SetSlidingExpiration(TimeSpan.FromMinutes(5)); // Reset each 5 minutes
            _memoryCache.Set(cacheKey, cachedData, cacheEntryOptions);

        }
        return cachedData;
    }

    public async Task<List<ChapterDTO>?> GetChapters(int comicid)
    {
        // DateTime start = DateTime.Now;
        var chapters = await _dbContext.Chapters
        .AsNoTracking()
        .Where(x => x.ComicID == comicid)
        .OrderByDescending(x => x.Url)
        .Select(x => new ChapterDTO(x))
        .ToListAsync();
        // TimeSpan timeItTook = DateTime.Now - start;
        // Console.WriteLine(value: $"GetChapters took {timeItTook.TotalMilliseconds} ms");
        return chapters;

    }

    public async Task<List<ComicDTO>> GetComicByKeyword(string keyword)
    {
        Dictionary<int, Comic>? comis = await GetAllComics();
        keyword = SlugHelper.CreateSlug(keyword);
        List<List<string>> ngrams = new List<List<string>> { JaccardSearch.GetNgrams(keyword, 2, false), JaccardSearch.GetNgrams(keyword, 3, false), JaccardSearch.GetNgrams(keyword, 4, false) };
        List<(Comic comic, double similarity)> result = new List<(Comic, double)>();
        List<string> comicNames = new List<string>();
        // Console.WriteLine($"GetComicByKeyword: {CalculateDictionaryMemory(SlugHelper._slugCache)}");
        foreach (KeyValuePair<int, Comic> comic in comis)
        {
            comicNames.Clear();
            comicNames.Add(comic.Value.Title);
            double similarity = 0;
            if (!string.IsNullOrEmpty(comic.Value.OtherName))
            {
                var otherNames = comic.Value.OtherName.Split(";", StringSplitOptions.RemoveEmptyEntries);
                comicNames.AddRange(otherNames);
            }
            for (int i = 0; i < comicNames.Count; i++)
            {
                string slugComicName = SlugHelper.CreateSlug(comicNames[i], true, true);
                int n = JaccardSearch.CalNGram(slugComicName);
                similarity = Math.Max(similarity, JaccardSearch.CalculateSimilarity(ngrams[n - 2], JaccardSearch.GetNgrams(slugComicName, n, true)));
                if (i == 0) similarity += 0.05;
            }

            if (similarity > 0.1)
            {
                result.Add((comic.Value, similarity));
            }
        }

        return result
        .OrderByDescending(x => x.similarity)
        .Take(5)
        .Select(x => new ComicDTO(x.comic, _urlService)).ToList();
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
        .Include(x => x.Genres)
        .Select(x => new ComicDTO(x, _urlService)
        {
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

        return new ListComicDTO()
        {
            totalpage = 0,
            Page = page,
            Step = step,
            comics = new List<ComicDTO>()

        };
    }
    private async Task<List<ComicDTO>?> _getComicRecommendFromDB()
    {
        var comicsQuery = _dbContext.Comics.AsNoTracking().AsQueryable();
        // Áp dụng bộ lọc loại trừ thể loại (Nogenres)
        var nogenreIds = NoGenres.ToHashSet();
        var datenow = DateTime.UtcNow;
        comicsQuery = comicsQuery.Where(x => !x.Genres.Select(g => g.Slug).Any(gSlug => nogenreIds.Contains(gSlug)));
        comicsQuery = comicsQuery.Where(x => (datenow - x.UpdateAt).TotalDays <= 700);
        comicsQuery = comicsQuery.Where(x => x.numchapter > 10);
        var data = await comicsQuery
        .OrderByDescending(x => x.ViewCount)
        .Include(x => x.Genres)
        .Take(2000)
        .ToListAsync();
        List<float> data2 = data.Select(x => (float)x.ViewCount).ToList();
        data = ServiceUtilily.SampleList(data, data2, 30);
        var result = data.Select(x => new ComicDTO(x, _urlService)).ToList();

        return result;
    }

    public async Task<List<ComicDTO>?> GetComicRecommend()
    {

        const string cacheKey = "RECOMMEND_COMIC_KEY";
        if (!CacheEnabled || !_memoryCache.TryGetValue(cacheKey, out List<ComicDTO>? cachedData))
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
        .Include(x => x.Genres)
        .Select(x => new ComicDTO(x, _urlService)
        {
            Chapters = x.Chapters.Where(c => c.ID == x.lastchapter).Select(ch => new ChapterDTO(ch))
        }).ToListAsync();
        var weeklyComics = await _dbContext.GetTopDailyComics(TopViewType.Week)
        .Take(step)
        .Include(x => x.Genres)
        .Select(x => new ComicDTO(x, _urlService)
        {
            Chapters = x.Chapters.Where(c => c.ID == x.lastchapter).Select(ch => new ChapterDTO(ch))
        })
        .ToListAsync();
        var monthlyComics = await _dbContext.GetTopDailyComics(TopViewType.Month)
        .Take(step)
        .Include(x => x.Genres)
        .Select(x => new ComicDTO(x, _urlService)
        {
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
        if (!CacheEnabled || !_memoryCache.TryGetValue(cacheKey, out ComicTopViewDTO? cachedData))
        {
            cachedData = await _getTopViewComicsFromDB(step);
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                 .SetSlidingExpiration(TimeSpan.FromMinutes(10)); // Reset each 10 minutes
            _memoryCache.Set(cacheKey, cachedData, cacheEntryOptions);
        }
        return cachedData!;
    }

    public async Task<bool> UpdateViewComic(HashSet<int> comicview)
    {

        try
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

            return true;
        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            Console.WriteLine($"Transaction rolled back due to: {ex.Message}");
            return false;
        }
    }
    public async Task<bool> UpdateViewChapter(Dictionary<int, int> chapterviews)
    {

        try
        {
            var chapterIds = chapterviews.Keys.ToList();
            var chaptersToUpdate = _dbContext.Chapters
                                                .Where(c => chapterIds.Contains(c.ID));

            foreach (var chapter in chaptersToUpdate)
            {
                chapter.ViewCount += chapterviews[chapter.ID];
            }
            await _dbContext.SaveChangesAsync();
            return true;

        }
        catch (Exception ex)
        {
            _dbContext.ChangeTracker.Clear();
            Console.WriteLine($"Transaction rolled back due to: {ex.Message}");

            return false;
        }
    }

    public async Task<List<ComicDTO>> GetComicsByIds(List<int> ids)
    {
        var comics = await _dbContext.Comics
        .Where(x => ids.Contains(x.ID))
        .Include(x => x.Genres)
        .Select(x => new ComicDTO(x, _urlService)
        {
            Chapters = x.Chapters.Where(c => c.ID == x.lastchapter).Select(ch => new ChapterDTO(ch))
        }).ToListAsync();
        return comics;
    }

    public async Task<List<ComicDTO>> FindSimilarComics(int comicid)
    {
        string cacheKey = $"SIMILAR_COMIC_KEY{comicid}";
        if (!CacheEnabled || !_memoryCache.TryGetValue(cacheKey, out List<ComicDTO>? cachedData))
        {
            cachedData = await _getSimilarComicsFromDB(comicid, 12);
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                 .SetSlidingExpiration(TimeSpan.FromMinutes(5)); // Reset each 10 minutes
            _memoryCache.Set(cacheKey, cachedData, cacheEntryOptions);
        }
        return cachedData!;
    }

    private async Task<List<ComicDTO>> _getSimilarComicsFromDB(int comicid, int size)
    {
        Dictionary<int, Comic>? _comics = await GetAllComics();
        if (!_comics.TryGetValue(comicid, out Comic? _comic))
        {
            return new List<ComicDTO>();
        }
        IEnumerable<int>? _genres = _comic.Genres.Select(x => x.ID);

        List<Comic> result = new List<Comic>();
        Dictionary<int, List<Comic>> dictKey = new Dictionary<int, List<Comic>>();
        int minElement = Math.Min(3, _genres.Count());
        foreach (var comic in _comics.Values)
        {
            if (comic.ID == comicid) continue;
            IEnumerable<int>? genre = comic.Genres.Select(x => x.ID);

            int countElement = _genres.Intersect(genre).Sum(x => 1);
            if (countElement >= minElement)
            {
                if (!dictKey.ContainsKey(countElement))
                {
                    dictKey.Add(countElement, new List<Comic>());
                }
                dictKey[countElement].Add(comic);
            }
        }
        List<int>? keys = dictKey.Keys.ToList();
        //Sort the keys in descending order
        keys.Sort((x, y) => y.CompareTo(x));
        for (int i = 0; i < keys.Count; i++)
        {
            result.AddRange(dictKey[keys[i]]);
            if (result.Count > 100) break;
        }
        ServiceUtilily.SuffleList(result);
        IEnumerable<Comic>? query = result.Where(x => x.UpdateAt > DateTime.Now.AddYears(-4)).Take(size);
        var resultDTO = query.Select(x => new ComicDTO(x, _urlService));
        return resultDTO.ToList();
    }

    public async Task<List<AnnouncementDTO>?> GetAnnouncements()
    {
        return await _dbContext.Announcements
            .Where(a => a.IsVisible && a.ApplyFrom <= DateTime.UtcNow && a.ApplyTo >= DateTime.UtcNow)
            .OrderByDescending(a => a.CreateAt)
            .Select(a => new AnnouncementDTO
            {
                Title = a.Title,
                Content = a.Content
            })
            .ToListAsync();

    }
}