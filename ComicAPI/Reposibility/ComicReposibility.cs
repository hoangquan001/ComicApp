using ComicAPI.Enums;
using ComicApp.Data;
using ComicApp.Models;
using Microsoft.EntityFrameworkCore;

public class ComicReposibility : IComicReposibility
{
    private readonly ComicDbContext _dbContext;

    public ComicReposibility(ComicDbContext dbContext)
    {
        _dbContext = dbContext;

    }

    public Comic AddComic(Comic comic)
    {
        throw new NotImplementedException();
    }

    public async Task<ListComicDTO?> GetComics(int page, int step, int genre = -1, ComicStatus status = ComicStatus.All, SortType sort = SortType.TopAll)
    {
        var data = await _dbContext.Comics
        .Where(x => (status == ComicStatus.All || x.Status == (int)status) && (genre == -1 || x.Genres.Any(g => genre == g.ID)))
        .OrderComicByType(sort)
        .Select(x => new ComicDTO
        {
            ID = x.ID,
            Title = x.Title,
            OtherName = x.OtherName,
            Author = x.Author,
            Url = x.Url,
            Description = x.Description,
            Status = x.Status,
            Rating = x.Rating,
            UpdateAt = x.UpdateAt,
            CoverImage = x.CoverImage,
            ViewCount = x.ViewCount,
            genres = x.Genres.Select(g => new GenreLiteDTO { ID = g.ID, Title = g.Title }),
            Chapters = _dbContext.Chapters.Where(c =>c.ID == x.lastchapter).Select(ch => ChapterSelector(ch)).ToList()
        })
        .Skip((page - 1) * step)
        .Take(step)
        .ToListAsync();
        if (data != null)
        {
            int totalcomic = _dbContext.Comics.Where(x => (status == ComicStatus.All || x.Status == (int)status) && (genre == -1 || x.Genres.Any(g => genre == g.ID))).Count();
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
            ChapterNumber = x.ChapterNumber,
            ViewCount = x.ViewCount,
            UpdateAt = x.UpdateAt,
            Slug = x.Url
        };
    }
    public async Task<List<ComicDTO>> GetAllComics()
    {
        return await _dbContext.Comics
        .Select(x => new ComicDTO
        {
            ID = x.ID,
            Title = x.Title,
            OtherName = x.OtherName,
            Author = x.Author,
            Url = x.Url,
            Description = x.Description,
            Status = x.Status,
            Rating = x.Rating,
            UpdateAt = x.UpdateAt,
            CoverImage = x.CoverImage,
            ViewCount = x.ViewCount,
            genres = x.Genres.Select(g => new GenreLiteDTO { ID = g.ID, Title = g.Title }),
            Chapters = _dbContext.Chapters.Where(c => c.ID == x.lastchapter).Select(ch => ChapterSelector(ch)).ToList()
        }).OrderBy(x => x.ID).ToListAsync();


    }
    public List<Genre> GetGenres()
    {
        throw new NotImplementedException();
    }

    public byte[] LoadImage(string url)
    {
        throw new NotImplementedException();
    }

    public async Task<ComicDTO?> GetComic(string key)
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
            CoverImage = x.CoverImage,
            Description = x.Description,
            Status = x.Status,
            Rating = x.Rating,
            UpdateAt = x.UpdateAt,
            ViewCount = x.Chapters.Sum(x => x.ViewCount),
            genres = x.Genres.Select(g => new GenreLiteDTO { ID = g.ID, Title = g.Title }).ToList(),
            Chapters = x.Chapters.OrderByDescending(x => x.ChapterNumber).Select(x => ChapterSelector(x)).ToList()
        })
        .AsSplitQuery()
        .FirstOrDefaultAsync();

        return data;
    }

    public async Task<Chapter?> GetChapter(int chapter_id)
    {
        var chapter = await _dbContext.Chapters.Where(x => x.ID == chapter_id).FirstOrDefaultAsync();
        if (chapter == null) return null;
        // Comic? c = await _dbContext.Comics.Where(x => x.ID == chapter.ComicID).FirstOrDefaultAsync();
        // if (c == null) return null;
        // chapter.comic = c;
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
    public async Task<ListComicDTO?> GetUserFollowComics(int userid, int page, int size)
    {
        var data = await _dbContext.UserFollowComics
        .Where(x => x.UserID == userid)
        .Include(x => x.comic)
        .OrderByDescending(x => x.comic!.UpdateAt)
        .Select(x => new ComicDTO
        {
            ID = x.comic!.ID,
            Title = x.comic.Title,
            OtherName = x.comic.OtherName,
            Author = x.comic.Author,
            Url = x.comic.Url,
            Description = x.comic.Description,
            Status = x.comic.Status,
            Rating = x.comic.Rating,
            UpdateAt = x.comic.UpdateAt,
            CoverImage = x.comic.CoverImage,
            ViewCount = x.comic.ViewCount,
            genres = x.comic.Genres.Select(g => new GenreLiteDTO { ID = g.ID, Title = g.Title }).ToList(),
            Chapters = _dbContext.Chapters.Where(c => c.ID == x.comic.lastchapter).Select(ch => ChapterSelector(ch)).ToList()
        })
        .Skip((page - 1) * size)
        .Take(size).ToListAsync();
        if (data != null)
        {
            int totalcomic = _dbContext.UserFollowComics.Where(x => x.UserID == userid).Count();
            ListComicDTO list = new ListComicDTO
            {
                totalpage = (int)MathF.Ceiling((float)totalcomic / size),
                Page = page,
                Step = size,
                comics = data
            };
            return list;
        }
        return null;
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

    public async Task<ListComicDTO?> GetComicBySearchAdvance(SortType sort = SortType.TopAll, ComicStatus status = ComicStatus.All,
     List<int>? genres = null, int page = 1, int step = 100, List<int>? Nogenres = null)
    {


        genres ??= new List<int> { -1 };
        Nogenres ??= new List<int> { -1 };

        var comicsQuery = _dbContext.Comics.AsQueryable();

        // Áp dụng bộ lọc trạng thái
        if (status != ComicStatus.All)
        {
            comicsQuery = comicsQuery.Where(x => x.Status == (int)status);
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

        // Execute query and get data
        var data = await comicsQuery
            .OrderComicByType(sort)
            .Select(x => new ComicDTO
            {
                ID = x.ID,
                Title = x.Title,
                OtherName = x.OtherName,
                Author = x.Author,
                Url = x.Url,
                Description = x.Description,
                Status = x.Status,
                Rating = x.Rating,
                UpdateAt = x.UpdateAt,
                CoverImage = x.CoverImage,
                ViewCount = x.ViewCount,
                genres = x.Genres.Select(g => new GenreLiteDTO { ID = g.ID, Title = g.Title }),
                Chapters = x.Chapters.OrderByDescending(ch => ch.ChapterNumber).Select(ch => ChapterSelector(ch)).Take(1)
            })
            .Skip((page - 1) * step)
            .Take(step)
            .ToListAsync();

        // Get total count
        var totalComics = await comicsQuery.CountAsync();

        if (data != null && data.Any())
        {
            ListComicDTO list = new ListComicDTO
            {
                totalpage = (int)MathF.Ceiling((float)totalComics / step),
                comics = data
            };

            return list;
        }

        return null;
    }

    public async Task<List<ComicDTO>?> GetComicRecommend()
    {



        // var startTime = DateTime.Now;




        var Nogenres = new List<string> { "gender-bender","adult" ,"dam-my",
                                        "gender-bender","shoujo-ai","shounen-ai",
                                        "smut","soft-yaoi","soft-yuri"};

        var comicsQuery = _dbContext.Comics.AsQueryable();

        // Áp dụng bộ lọc loại trừ thể loại (Nogenres)

        var nogenreIds = Nogenres.ToHashSet();

        comicsQuery = comicsQuery.Where(x => !x.Genres.Select(g => g.Slug).Any(gSlug => nogenreIds.Contains(gSlug)));
        var datenow = DateTime.UtcNow;
        comicsQuery = comicsQuery.Where(x => (datenow - x.UpdateAt).TotalDays <= 365
         && x.ViewCount >= 100000 && x.Rating > 3.5);
        // query number chaper>10
        // comicsQuery=comicsQuery.Where(x=>x.numChapter>10);
        comicsQuery = comicsQuery.Where(x => x.Chapters.Count() > 10);


        // Execute query and get data
        var data = await comicsQuery
            // .OrderBy(x => Guid.NewGuid())
            .OrderBy(x => new Random().Next())
            .Select(x => new ComicDTO
            {
                ID = x.ID,
                Title = x.Title,
                OtherName = x.OtherName,
                Author = x.Author,
                Url = x.Url,
                Description = x.Description,
                Status = x.Status,
                Rating = x.Rating,
                UpdateAt = x.UpdateAt,
                CoverImage = x.CoverImage,
                ViewCount = x.ViewCount,
                genres = x.Genres.Select(g => new GenreLiteDTO { ID = g.ID, Title = g.Title }),
                Chapters = x.Chapters.OrderByDescending(ch => ch.ChapterNumber).Select(ch => ChapterSelector(ch)).Take(1)
            })

            .Take(50)
            .ToListAsync();

        // var time = DateTime.Now.Subtract(startTime);
        // Console.WriteLine("time run: " + time);
        if (data != null && data.Any())
        {
            return data;
        }


        return null;
    }
}