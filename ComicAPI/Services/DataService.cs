using ComicAPI.Enums;
using ComicApp.Data;
using ComicApp.Models;
using Microsoft.Extensions.Caching.Memory;

public class DataService : IDataService
{
    private readonly IComicReposibility _comicReposibility;
    private readonly IMemoryCache _cache;

    public DataService(IComicReposibility comicReposibility, IMemoryCache cache)
    {
        _comicReposibility = comicReposibility;
        _cache = cache;
    }

    public async Task<List<ComicDTO>> GetAllComic()
    {
        const string cacheKey = "ALL_COMIC_KEY";

        if (!_cache.TryGetValue(cacheKey, out List<ComicDTO>? cachedData))
        {
            cachedData = await _comicReposibility.GetAllComics();
            var cacheEntryOptions = new MemoryCacheEntryOptions()
                 .SetSlidingExpiration(TimeSpan.FromMinutes(10)); // Example: cache for 10 minutes
            _cache.Set(cacheKey, cachedData, cacheEntryOptions);
        }

        return cachedData!;
    }


    public ComicDTO AddComic(ComicDTO comic)
    {

        return comic;
    }

    //Implement all the methods of IComicReposibility

    public List<ComicDTO>? GetComics(string? search = null, ComicStatus? status = null, int page = 1, int pageSize = 10)
    {
        throw new NotImplementedException();
    }

    // public ComicDTO? GetComic(string key)
    // {
    //     if (_cache.ContainsKey(key))
    //     {
    //         return _cache[key];
    //     }
    //     return null;
    // }

    public List<Genre> GetGenres()
    {
        throw new NotImplementedException();
    }

    public byte[] LoadImage(string url)
    {
        throw new NotImplementedException();
    }

    public List<ComicDTO> GetComics(int page, int step, int genre = -1, ComicStatus status = ComicStatus.All, SortType sort = SortType.TopAll)
    {
        throw new NotImplementedException();
    }



    public Chapter? GetChapter(int chapter_id)
    {
        throw new NotImplementedException();
    }

    public List<ChapterDTO>? GetChaptersComic(string key)
    {
        throw new NotImplementedException();
    }

    public async Task<ComicDTO> GetComicByID(string id)
    {
        var data = await GetAllComic();
        bool IsInt = int.TryParse(id, out int id2);
        var result = data.FirstOrDefault(x => IsInt ? x.ID == id2 : x.Url == id)!;
        result.Chapters = await _comicReposibility.GetChaptersComic(result.Url) ?? new List<ChapterDTO>();
        return result;
    }
}

