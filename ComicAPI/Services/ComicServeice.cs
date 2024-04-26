
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
using ComicAPI.Classes;
using HtmlAgilityPack;
using System.Collections.Immutable;
using System.Net;
using System.Net.Http.Headers;
namespace ComicApp.Services;

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

    public async Task<ServiceResponse<ComicDTO>> GetComic(string key)
    {
        bool isID = int.TryParse(key, out int id2);


        var data = await _dbContext.Comics.Select(x => new ComicDTO
        {
            ID = x.ID,
            Title = x.Title,
            Author = x.Author,
            Url = x.Url,
            CoverImage = "https://static.doctruyenonline.vn/images/vu-luyen-dien-phong.jpg",
            Description = x.Description,
            Status = x.Status,
            Rating = x.Rating,
            UpdateAt = x.UpdateAt,
            ViewCount = x.Chapters.Sum(x => x.ViewCount),
            genres = x.Genres.Select(g => new GenreLiteDTO { ID = g.ID, Title = g.Title }).ToList(),
            Chapters = x.Chapters.OrderByDescending(x => x.ChapterNumber).Select(x => ChapterSelector(x)).ToList()
        })
        .Where(x => isID ? x.ID == id2 : x.Url == key)
        .AsSplitQuery()
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

    public async Task<ServiceResponse<List<ComicDTO>>> GetComics(ComicQueryParams comicQueryParams)
    {
        int page = comicQueryParams.page == 0 ? 1 : comicQueryParams.page;
        int step = comicQueryParams.step == 0 ? 10 : comicQueryParams.step;
        var data = await _dbContext.Comics

        .OrderComicByType(comicQueryParams.sort)
        .Select(x =>
        new ComicDTO
        {
            ID = x.ID,
            Title = x.Title,
            Author = x.Author,
            Url = x.Url,
            // CoverImage = "https://static.doctruyenonline.vn/images/vu-luyen-dien-phong.jpg",
            Description = x.Description,
            Status = x.Status,
            Rating = x.Rating,
            UpdateAt = x.UpdateAt,
            ViewCount = x.Chapters.Sum(x => x.ViewCount),
            genres = x.Genres.Select(g => new GenreLiteDTO { ID = g.ID, Title = g.Title }).ToList(),
            Chapters = x.Chapters.Select(x => ChapterSelector(x)).Take(2).ToList()
        })
        .Where(comicQueryParams.status == ComicStatus.All ? x => true : x => x.Status == (int)comicQueryParams.status)
        .Where(comicQueryParams.genre == -1 ? x => true : x => x.genres.Any(g => comicQueryParams.genre == g.ID))
        .Skip((page - 1) * step)
        .Take(step)
        // .AsSplitQuery()d
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
    private static readonly HttpClient _httpClient = new HttpClient();
    static async Task<List<PageDTO>> FetchChapterImage(string comic_slug, string chapter_slug, int chapterid)
    {
        List<PageDTO> urls = new List<PageDTO>();
        comic_slug = "truyen-tranh/het-nhu-han-quang-gap-nang-gat";
        chapter_slug = "chap-143.2";
        chapterid = 684968;
        string url = $"https://nhattruyenss.com/truyen-tranh/{comic_slug}/{chapter_slug}/{chapterid}";
        var request = new HttpRequestMessage()
        {
            RequestUri = new Uri(url),
            Method = HttpMethod.Get,
        };
        string customUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/111.0.0.0 Safari/537.36";
        // request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/plain"));
        var response = await _httpClient.SendAsync(request);
        System.Threading.Thread.Sleep(1000);
        string responseBody = await response.Content.ReadAsStringAsync();

        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(responseBody);
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
    public async Task<ServiceResponse<ChapterPageDTO>> GetPagesInChapter(int comic_id, int chapter_id)
    {
        var chapter = await _dbContext.Chapters.Where(x => x.ID == chapter_id).FirstOrDefaultAsync();
        List<PageDTO>? urlsData = null;
        if (chapter != null)
        {
        }
        urlsData = await FetchChapterImage("a", "b", chapter_id);


        ServiceResponse<ChapterPageDTO> res = new ServiceResponse<ChapterPageDTO>();
        res.Data = new ChapterPageDTO()
        {
            Pages = urlsData
        };
        return res;
    }

    public async Task<ServiceResponse<List<ChapterDTO>>> GetChaptersComic(string key)
    {
        bool isID = int.TryParse(key, out int id2);
        var data = await _dbContext.Comics.Where(x => isID ? x.ID == id2 : x.Url == key)
        .Select(x => x.Chapters.Select(x => ChapterSelector(x)).ToList())
        .FirstOrDefaultAsync();
        return GetDataRes<List<ChapterDTO>>(data);

    }
}
