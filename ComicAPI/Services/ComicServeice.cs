
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
using Microsoft.EntityFrameworkCore.Internal;
namespace ComicApp.Services;
public class ComicService : IComicService
{
    readonly ComicDbContext _dbContext;
    readonly IMapper _mapper;
    private static readonly HttpClient _httpClient = new HttpClient();


    public ComicService(ComicDbContext db, IMapper mapper)
    {
        _dbContext = db;
        _mapper = mapper;
    }

    public async Task<ServiceResponse<ComicDTO>> GetComic(string key)
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
        return GetDataRes<ComicDTO>(data);
    }


    public async Task<ServiceResponse<List<ComicDTO>>> GetComics(ComicQueryParams comicQueryParams)
    {
        int page = comicQueryParams.page < 1 ? 1 : comicQueryParams.page;
        int step = comicQueryParams.step < 1 ? 10 : comicQueryParams.step;
        var data = await _dbContext.Comics
           .Where(x =>
               (comicQueryParams.status == ComicStatus.All || x.Status == (int)comicQueryParams.status) &&
               (comicQueryParams.genre == -1 || x.Genres.Any(g => comicQueryParams.genre == g.ID)))
            .OrderComicByType(comicQueryParams.sort)
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
        return GetDataRes<List<ComicDTO>>(data);

    }

    public async Task<ServiceResponse<Comic>> AddComic(Comic comic)
    {
        _dbContext.Comics.Add(comic);
        await _dbContext.SaveChangesAsync();
        return GetDataRes<Comic>(comic);
    }

    public async Task<ServiceResponse<List<Genre>>> GetGenres()
    {
        var data = await _dbContext.Genres.ToListAsync();
        return GetDataRes<List<Genre>>(data);
    }
    static async Task<List<PageDTO>?> FetchChapterImage(string comic_slug, string chapter_slug, int chapterid)
    {
        List<PageDTO> urls = new List<PageDTO>();
        string url = $"https://nhattruyenss.com/truyen-tranh/{comic_slug}/{chapter_slug}/{chapterid}";
        var request = new HttpRequestMessage();
        request.RequestUri = new Uri(url);
        request.Method = HttpMethod.Get;

        request.Headers.Add("Accept", "*/*");
        request.Headers.Add("User-Agent", "Thunder Client (https://www.thunderclient.com)");

        var response = await _httpClient.SendAsync(request);
        string responseBody = await response.Content.ReadAsStringAsync();

        HtmlDocument doc = new HtmlDocument();
        doc.LoadHtml(responseBody);
        HtmlNodeCollection elements = doc.DocumentNode.SelectNodes("//div[contains(@class, 'page-chapter')]");
        if (elements == null) return null;
        int i = 0;
        Random rnd = new Random();
        foreach (HtmlNode element in elements)
        {
            string imgUrl = "https:" + element.SelectSingleNode("img").GetAttributeValue("src", "");
            string data = ServiceUtils.Base64Encode(imgUrl);
            int num = rnd.Next();
            PageDTO page = new PageDTO()
            {
                URL = $"http://localhost:5080/data/img/{num}.jpg?data={data}",
                PageNumber = ++i,

            };
            urls.Add(page);
            // Add imgUrl to list or perform other tasks
        }
        return urls;
    }
    public async Task<byte[]> LoadImage(string url)
    {
        var request = new HttpRequestMessage();
        request.RequestUri = new Uri(url);
        request.Method = HttpMethod.Get;
        request.Headers.Add("Accept", "*/*");
        request.Headers.Add("User-Agent", "Thunder Client (https://www.thunderclient.com)");
        request.Headers.Add("Referer", "nhattruyenss.com");
        var response = await _httpClient.SendAsync(request);
        var imgByte = await response.Content.ReadAsByteArrayAsync();
        return imgByte;
    }
    public async Task<ServiceResponse<ChapterPageDTO>> GetPagesInChapter(int chapter_id)
    {
        var chapter = await _dbContext.Chapters.Where(x => x.ID == chapter_id).FirstOrDefaultAsync();
        if (chapter == null) return GetDataRes<ChapterPageDTO>(null);

        chapter.comic = await _dbContext.Comics.Where(x => x.ID == chapter.ComicID).FirstOrDefaultAsync();
        if (chapter.comic == null) return GetDataRes<ChapterPageDTO>(null);
        List<PageDTO>? urlsData = null;
        if (chapter != null)
        {
            urlsData = await FetchChapterImage(chapter.comic.Url, chapter.Url, chapter_id);
        }
        ChapterPageDTO chapterPageDTO = new ChapterPageDTO { Pages = urlsData };
        return GetDataRes<ChapterPageDTO>(chapterPageDTO);
    }

    public async Task<ServiceResponse<List<ChapterDTO>>> GetChaptersComic(string key)
    {
        bool isID = int.TryParse(key, out int id2);
        var data = await _dbContext.Comics.Where(x => isID ? x.ID == id2 : x.Url == key)
        .Select(x => x.Chapters.Select(x => ChapterSelector(x)).ToList())
        .FirstOrDefaultAsync();
        return GetDataRes<List<ChapterDTO>>(data);

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
}
