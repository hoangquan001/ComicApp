
using ComicApp.Data;
using ComicApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ComicAPI.Enums;
using System.Linq.Expressions;
using AutoMapper;
using HtmlAgilityPack;
using System.Collections.Immutable;
namespace ComicApp.Services;
static class DbSetExtension
{
    public static IQueryable<Comic> OrderComicByType(this IQueryable<Comic> query, SortType sortType)
    {
        switch (sortType)
        {
            case SortType.Chapter:
                return query.OrderByDescending(x => x.Chapters.Count);
            case SortType.LastUpdate:
                return query.OrderByDescending(keySelector: x => x.UpdateAt);
            // case SortType.TopFollow:
            //     return query.OrderBy(x => x.Follow);
            // case SortType.TopComment:
            //     return query.OrderBy(x => x.Comment);
            case SortType.NewComic:
                return query.OrderBy(x => x.Chapters);
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
public class ComicService : IComicService
{
    readonly ComicDbContext _dbContext;
    readonly IMapper _mapper;
    //Contructor
    public ComicService(ComicDbContext db, IMapper mapper)
    {
        _dbContext = db;
        _mapper = mapper;
    }

    // Get one comic

    public async Task<ServiceResponse<ComicDTO>> GetComic(int id)
    {
        var data = await _dbContext
        .Comics
        .AsNoTracking()
        .Where(x => x.ID == id)
        .Select(x => new ComicDTO()
        {
            ID = x.ID,
            Title = x.Title,
            Author = x.Author,
            CoverImage = x.CoverImage,
            Description = x.Description,
            Status = x.Status,
            Rating = x.Rating,
            UpdateAt = x.UpdateAt,
            ViewCount = x.Chapters.Sum(x => x.ViewCount),
            genres = x.genres.Select(g => new GenreLiteDTO { ID = g.ID, Title = g.Title }),
            Chapters = x.Chapters.Select(x => new ChapterDTO
            {
                ID = x.ID,
                Title = x.Title,
                ChapterNumber = x.ChapterNumber,
                ViewCount = x.ViewCount,
                UpdateAt = x.UpdateAt
            })
        }
        )
        .FirstOrDefaultAsync();
        return GetDataRes<ComicDTO>(data);
    }

    public ServiceResponse<T> GetDataRes<T>(T? data)
    {
        var res = new ServiceResponse<T>();

        if (data == null)
        {
            res.Status = 0;
            res.Message = "Not found";

        }
        else
        {
            res.Data = data;
            res.Status = 1;
            res.Message = "Success";
        }

        return res;

    }
    public async Task<ServiceResponse<List<ComicDTO>>> GetComics(int page, int step, SortType sortType = SortType.TopAll)

    {
        if (page < 1) page = 1;
        // _dbContext.Comics.Load();
        var data = await _dbContext.Comics
        .OrderComicByType(SortType.TopAll)
        .Select(x => new ComicDTO
        {
            ID = x.ID,
            Title = x.Title,
            Author = x.Author,
            CoverImage = x.CoverImage,
            Description = x.Description,
            Status = x.Status,
            Rating = x.Rating,
            UpdateAt = x.UpdateAt,
            ViewCount = x.Chapters.Sum(x => x.ViewCount),
            genres = x.genres.Select(g => new GenreLiteDTO { ID = g.ID, Title = g.Title }),
            Chapters = x.Chapters.Select(x => new ChapterDTO
            {
                ID = x.ID,
                Title = x.Title,
                ChapterNumber = x.ChapterNumber,
                ViewCount = x.ViewCount,
                UpdateAt = x.UpdateAt
            }).Take(3)
        })
        .Skip((page - 1) * step)
        .Take(step)
        .ToListAsync();
        return new ServiceResponse<List<ComicDTO>>
        {
            Data = data,
            Status = 1,
            Message = "Success"
        };
    }

    public async Task<ServiceResponse<Comic>> AddComic(Comic comic)
    {
        _dbContext.Comics.Add(comic);
        await _dbContext.SaveChangesAsync();
        return new ServiceResponse<Comic>
        {
            Data = comic,
            Status = 1,
            Message = "Success"
        };
    }

    public async Task<ServiceResponse<List<Genre>>> GetGenres()
    {
        // await _dbContext.Genres.ToListAsync();
        return new ServiceResponse<List<Genre>>
        {
            Data = await _dbContext.Genres.ToListAsync(),
            Status = 1,
            Message = "Success"
        };
    }

    public async Task<ServiceResponse<List<Comic>>> GetComicsByGenre(int genre, int page, int step)
    {
        if (page < 1) page = 1;
        var data = await _dbContext.Comics
        .Skip((page - 1) * step)
        .Take(step)
        .Where(x => x.genres.Any(g => g.ID == genre))
        .Include(x => x.genres)
        .AsSplitQuery()
        .Include(x => x.Chapters)
        .OrderComicByType(SortType.TopAll)
        .ToListAsync();

        return new ServiceResponse<List<Comic>>
        {
            Data = data,
            Status = 1,
            Message = "Success"
        };
    }
    static async Task<List<PageDTO>> FetchChapterImage(IHeaderDictionary header)
    {
        List<PageDTO> urls = new List<PageDTO>();
        string url = "https://nhattruyenbing.com/truyen-tranh/mousou-sensei/chap-1/1140053";
        HtmlWeb web = new HtmlWeb();
        HtmlDocument doc = await web.LoadFromWebAsync(url);
        HtmlNodeCollection elements = doc.DocumentNode.SelectNodes("//div[contains(@class, 'page-chapter')]");
        int i = 0;
        foreach (HtmlNode element in elements)
        {
            string imgUrl = "https:" + element.SelectSingleNode("img").GetAttributeValue("src", "");
            PageDTO page = new PageDTO()
            {
                URL = imgUrl,
                PageNumber = ++i,

            };
            urls.Add(page);
            // Add imgUrl to list or perform other tasks
        }
        return urls;
    }
    public async Task<ServiceResponse<ChapterPageDTO>> GetPagesInChapter(IHeaderDictionary header, int comic_id, int chapter_id)
    {
        List<PageDTO> urlsData = await FetchChapterImage(header);
        ServiceResponse<ChapterPageDTO> res = new ServiceResponse<ChapterPageDTO>();
        res.Data = new ChapterPageDTO()
        {
            Pages = urlsData
        };
        return res;
    }
}