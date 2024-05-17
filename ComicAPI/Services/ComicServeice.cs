
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
using ComicAPI.Services;
namespace ComicApp.Services;
public class ComicService : IComicService
{
    readonly IDataService _dataService;
    readonly IComicReposibility _comicReposibility;
    readonly IMapper _mapper;
    private static readonly HttpClient _httpClient = new HttpClient();


    public ComicService(IComicReposibility comicReposibility, IMapper mapper, IDataService dataService)
    {
        _comicReposibility = comicReposibility;

        _mapper = mapper;

        _dataService = dataService;
    }

    public async Task<ServiceResponse<ComicDTO>> GetComic(string key, int maxchapter = -1)
    {
        var data = await _comicReposibility.GetComic(key);
        // if (data == null)
        // {
        // var data = await _dataService.GetComicByID(key);
        //     if (data != null)
        //     {
        //         _comicCacheReposibility.AddComic(data);
        //     }
        // }
        return ServiceUtilily.GetDataRes<ComicDTO>(data);
    }

    public async Task<ServiceResponse<List<ComicDTO>>> SearchComicByKeyword(string keyword)
    {
        var data = await _dataService.GetAllComic();
        var listkeys = SlugHelper.GetListKey(keyword);
        List<(ComicDTO comic, int count)> result = new List<(ComicDTO, int)>();
        Dictionary<int, int> myDict = new Dictionary<int, int>();
        for (int i = 0; i < data.Count; i++)
        {
            var listtitlekeys = SlugHelper.GetListKey(data[i].Title);

            var Elements = listkeys.Intersect(listtitlekeys);
            int countElement = Elements.Count();
            if (countElement > 0)
            {
                if (!myDict.ContainsKey(countElement))
                {
                    myDict[countElement] = 1;
                    result.Add((data[i], countElement));
                }
                else if (myDict[countElement] < 5)
                {
                    myDict[countElement]++;
                    result.Add((data[i], countElement));
                }

            }
        }
        result.Sort((x, y) => y.count.CompareTo(x.count));
        List<ComicDTO> result2 = result.Take(5).Select(x => x.comic).ToList();
        return ServiceUtilily.GetDataRes<List<ComicDTO>>(result2);
    }


    public async Task<ServiceResponse<ListComicDTO>> GetComics(ComicQueryParams comicQueryParams)
    {
        int page = comicQueryParams.page < 1 ? 1 : comicQueryParams.page;
        int step = comicQueryParams.step < 1 ? 10 : comicQueryParams.step;
        var data = await _comicReposibility.GetComics(page, step, comicQueryParams.genre, comicQueryParams.status, comicQueryParams.sort);

        return ServiceUtilily.GetDataRes<ListComicDTO>(data);

    }

    public async Task<ServiceResponse<Comic>> AddComic(Comic comic)
    {
        // _dbContext.Comics.Add(comic);
        // await _dbContext.SaveChangesAsync();
        await Task.Delay(1000);
        return ServiceUtilily.GetDataRes<Comic>(null);
    }

    public async Task<ServiceResponse<List<Genre>>> GetGenres()
    {
        // var data = await _dbContext.Genres.ToListAsync();
        await Task.Delay(1000);
        return ServiceUtilily.GetDataRes<List<Genre>>(null);
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
            string data = ServiceUtilily.Base64Encode(imgUrl);
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

        var chapter = await _comicReposibility.GetChapter(chapter_id);
        if (chapter == null) return ServiceUtilily.GetDataRes<ChapterPageDTO>(null);
        var comic = await _comicReposibility.GetComic(chapter.ComicID.ToString());
        if (comic == null)
        {
            return ServiceUtilily.GetDataRes<ChapterPageDTO>(null);
        }

        List<PageDTO>? urlsData = await FetchChapterImage(comic.Url, chapter.Url, chapter_id);
        ChapterPageDTO chapterPageDTO = new ChapterPageDTO
        {
            ID = chapter.ID,
            Title = chapter.Title,
            Slug = chapter.Url,
            ChapterNumber = chapter.ChapterNumber,
            UpdateAt = chapter.UpdateAt,
            ViewCount = chapter.ViewCount,
            Pages = urlsData,
            Comic = comic
        };
        return ServiceUtilily.GetDataRes<ChapterPageDTO>(chapterPageDTO);
    }

    public async Task<ServiceResponse<List<ChapterDTO>>> GetChaptersComic(string key)
    {
        var data = await _comicReposibility.GetChaptersComic(key);
        return ServiceUtilily.GetDataRes<List<ChapterDTO>>(data);

    }
    

}
