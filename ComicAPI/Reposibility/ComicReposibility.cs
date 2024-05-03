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
           .Skip((page - 1) * step)
           .Take(step)
           .Select(x => new ComicDTO
           {
               ID = x.ID,
               Title = x.Title,
               Author = x.Author,
               Url = x.Url,
               Description = x.Description,
               Status = x.Status,
               Rating = x.Rating,
               UpdateAt = x.UpdateAt,
               CoverImage = x.CoverImage,
               ViewCount = x.ViewCount,
               genres = x.Genres.Select(g => new GenreLiteDTO { ID = g.ID, Title = g.Title }),
               Chapters = x.Chapters.Select(ch => ChapterSelector(ch)).Take(1)
           })
            .ToListAsync();
        if (data != null)
        {
            int totalcomic = _dbContext.Comics.Where(x => (status == ComicStatus.All || x.Status == (int)status) && (genre == -1 || x.Genres.Any(g => genre == g.ID))).Count();
            ListComicDTO list = new ListComicDTO
            {
                totalpage = (int)MathF.Ceiling((float)totalcomic / step),
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
            Author = x.Author,
            Url = x.Url,
            Description = x.Description,
            Status = x.Status,
            Rating = x.Rating,
            UpdateAt = x.UpdateAt,
            CoverImage = x.CoverImage,
            ViewCount = x.ViewCount,
            genres = x.Genres.Select(g => new GenreLiteDTO { ID = g.ID, Title = g.Title }),
        }).ToListAsync();
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


        return result;
    }
}