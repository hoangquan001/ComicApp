
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
using System.Text.Json;

namespace ComicApp.Services;
public class ComicService : IComicService
{
    private static readonly HttpClient _httpClient = new HttpClient();
    private readonly IComicReposibility _comicReposibility;
    private static List<int> genreWeight = new List<int>();
    private readonly IUserService _userService;
    private readonly IMapper _mapper;
    private static Dictionary<int, int> views = new Dictionary<int, int>();

    static ComicService()
    {
        for (int i = 0; i < 55; i++)
        {
            genreWeight.Add(1);
        }
        // var data = GlobalConfig.GetString("genresWeight");
        // Console.WriteLine(data);
    }

    public ComicService(IComicReposibility comicReposibility, IMapper mapper
        , IUserService userService)
    {
        _comicReposibility = comicReposibility;
        _mapper = mapper;
        _userService = userService;

    }



    public async Task<ServiceResponse<ComicDTO>> GetComic(string key, int maxchapter = -1)
    {
        var data = await _comicReposibility.GetComic(key);
        if (data != null && _userService.UserID != -1)
        {
            data.IsFollow = await _userService.IsFollowComic(_userService.UserID, data.ID);
        }
        return ServiceUtilily.GetDataRes<ComicDTO>(data);
    }
    private List<ComicDTO> GetComicByKeyword(List<ComicDTO> data, string keyword)
    {
        keyword = keyword.Replace("-", " ");
        var listkeys = SlugHelper.GetListKey(keyword);
        List<(ComicDTO comic, int count)> result = new List<(ComicDTO, int)>();
        Dictionary<int, int> myDict = new Dictionary<int, int>();
        for (int i = 0; i < data.Count; i++)
        {
            var listtitlekeys = data[i].Url.Split('-');
            IEnumerable<string>? merge = listtitlekeys;
            if (data[i].OtherName != "")
            {
                var listotherkeys = SlugHelper.GetListKey(data[i].OtherName);
                merge = merge.Union(listotherkeys);
            }
            int countElement = 0;
            if (listkeys.Count == 1)
            {
                string f = listkeys.First();
                if (listtitlekeys.Contains(f))
                {
                    countElement = 2;
                }
                else if (listtitlekeys.Any(x => x.Contains(f)))
                {
                    countElement = 1;
                }
            }
            else
            {
                var Elements = listkeys.Intersect(merge);
                countElement = Elements.Count();
            }

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
        return result2;
    }
    public async Task<ServiceResponse<List<ComicDTO>>> SearchComicByKeyword(string keyword)
    {
        var data = await _comicReposibility.GetAllComics();
        var result = GetComicByKeyword(data, keyword);
        return ServiceUtilily.GetDataRes<List<ComicDTO>>(result);

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
        try
        {
            chapter_slug = chapter_slug.Replace("chapter", "chuong");
            string url = $"https://nettruyenviet.com/truyen-tranh/{comic_slug}/{chapter_slug}";
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
                string imgUrl = element.SelectSingleNode("img").GetAttributeValue("data-src", "");
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

        }
        catch (Exception ex)
        {
            Console.WriteLine("Error fetching chapter images: " + ex.Message);
            return urls;
        }
        return urls;
    }
    public async Task<byte[]> LoadImage(string url)
    {
        HttpRequestMessage? request = new HttpRequestMessage();
        request.RequestUri = new Uri(url);
        request.Method = HttpMethod.Get;
        request.Headers.Add("Accept", "*/*");
        request.Headers.Add("User-Agent", "Thunder Client (https://www.thunderclient.com)");
        request.Headers.Add("Referer", "nettruyenviet.com");
        HttpResponseMessage? response = await _httpClient.SendAsync(request);
        byte[]? imgByte = await response.Content.ReadAsByteArrayAsync();
        return imgByte;
    }
    public async Task<ServiceResponse<ChapterPageDTO>> GetPagesInChapter(int chapter_id)
    {

        Chapter? chapter = await _comicReposibility.GetChapter(chapter_id);
        if (chapter == null) return ServiceUtilily.GetDataRes<ChapterPageDTO>(null);
        ComicDTO? comic = await _comicReposibility.GetComic(chapter.ComicID.ToString());
        if (comic == null) return ServiceUtilily.GetDataRes<ChapterPageDTO>(null);
        // List<PageDTO>? urlsData = await FetchChapterImage(comic.Url, chapter.Url, chapter_id);
        List<PageDTO>? urlsData = null;
        if (chapter.Pages != null)
        {
            List<string>? links = JsonSerializer.Deserialize<List<string>>(chapter.Pages.Replace("'", "\""));
            if (links != null)
            {
                urlsData = links.Select((x, i) => new PageDTO { URL = x, PageNumber = i }).ToList();
            }
        }

        ChapterPageDTO chapterPageDTO = new ChapterPageDTO
        {
            ID = chapter.ID,
            Title = chapter.Title,
            Slug = chapter.Url,
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

    public async Task<ServiceResponse<List<ComicDTO>>> GetSimilarComics(string key)
    {

        ServiceResponse<List<ComicDTO>>? response = await SearchComicByKeyword(key);
        foreach (var item in response.Data!)
        {
            item.Chapters = await _comicReposibility.GetChaptersComic(item.ID.ToString()) ?? [];
        }

        return response;
    }
    public async Task<ServiceResponse<List<ComicDTO>>> FindSimilarComicsAsync(int id)
    {
        var _comics = await _comicReposibility.GetAllComics();
        var _comic = _comics.FirstOrDefault(x => x.ID == id);
        var _genre = _comic!.genres.Select(x => x.ID);
        List<ComicDTO> result = new List<ComicDTO>();
        Dictionary<int, List<ComicDTO>> dictKey = new Dictionary<int, List<ComicDTO>>();
        int minElement = Math.Min(3, _genre.Count());
        for (int i = 0; i < _comics.Count; i++)
        {
            var comic = _comics[i];
            if (comic.ID == id) continue;
            var genre = comic.genres.Select(x => x.ID);

            int countElement = _genre.Intersect(genre).Sum(x => genreWeight[x]);
            if (countElement >= minElement)
            {
                if (!dictKey.ContainsKey(countElement))
                {
                    dictKey.Add(countElement, new List<ComicDTO>());
                }
                dictKey[countElement].Add(comic);
            }
        }
        var keys = dictKey.Keys.ToList();
        //Sort the keys in descending order
        keys.Sort((x, y) => y.CompareTo(x));
        foreach (var key in keys)
        {
            result.AddRange(dictKey[key]);
            if (result.Count > 200) break;
        }
        ServiceUtilily.SuffleList(result);
        result = result.Where(x => x.UpdateAt > DateTime.Now.AddYears(-2)).Take(12).ToList();

        return ServiceUtilily.GetDataRes<List<ComicDTO>>(result);

    }
    private List<int>? ParseGenreQuery(string? query)
    {
        if (string.IsNullOrEmpty(query)) return null;
        List<int> querys = new List<int>();
        string[] genreStrings = query.Split(',');
        foreach (string genreString in genreStrings)
        {
            if (int.TryParse(genreString, out int genre))
            {
                querys.Add((int)genre);
            }
        }
        return querys;
    }
    public async Task<ServiceResponse<ListComicDTO>> GetComicBySearchAdvance(ComicQuerySearchAdvance query)
    {


        int page = query.Page < 1 ? 1 : query.Page;
        int step = query.Step < 1 ? 10 : query.Step;
        var Genres = ParseGenreQuery(query.Genres);
        var Notgenres = ParseGenreQuery(query.Notgenres);
        var data = await _comicReposibility.GetComicBySearchAdvance(query.Sort, query.Status, Genres, page, step, Notgenres, query.Year, query.Keyword);
        return ServiceUtilily.GetDataRes<ListComicDTO>(data);

    }

    public async Task<ServiceResponse<List<ComicDTO>>> GetComicRecommend()
    {
        var data = await _comicReposibility.GetComicRecommend();
        return ServiceUtilily.GetDataRes<List<ComicDTO>>(data);
    }

    public async Task<ServiceResponse<ComicTopViewDTO?>> GetTopViewComics(int step)
    {
        var data = await _comicReposibility.GetTopViewComics(step);
        return ServiceUtilily.GetDataRes<ComicTopViewDTO>(data)!;
    }
    public Task<ServiceResponse<int>> TotalViewComics(int comicid)
    {
        if (!views.ContainsKey(comicid))
        {
            views[comicid] = 0;
        }
        views[comicid]++;

        return Task.FromResult(ServiceUtilily.GetDataRes<int>(views[comicid]));
    }

    public async Task UpdateViewComic()
    {
        await _comicReposibility.UpdateViewComic(views);
    }
}
