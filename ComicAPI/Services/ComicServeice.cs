
using ComicApp.Models;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using ComicAPI.Classes;
using HtmlAgilityPack;
using System.Collections.Immutable;
using System.Text.Json;
using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using System.Text;
using ComicAPI.Services;

namespace ComicApp.Services;
public class ComicService : IComicService
{
    private readonly HttpClient _httpClient;
    private readonly IComicReposibility _comicReposibility;
    private static List<int> genreWeight = new List<int>();
    private readonly IUserService _userService;
    private readonly IMapper _mapper;
    private readonly UrlService _urlService;
    private static ConcurrentDictionary<int, int> chapterViews = new ConcurrentDictionary<int, int>();
    private static ConcurrentDictionary<int, int> comicViews = new ConcurrentDictionary<int, int>();
    private readonly AppSetting _config;

    static ComicService()
    {
        for (int i = 0; i < 55; i++)
        {
            genreWeight.Add(1);
        }
        // var data = GlobalConfig.GetString("genresWeight");
        // Console.WriteLine(data);
    }

    public ComicService(IComicReposibility comicReposibility, IMapper mapper, UrlService urlService
        , IUserService userService, IOptions<AppSetting> options, HttpClient httpClient)
    {
        _comicReposibility = comicReposibility;
        _mapper = mapper;
        _userService = userService;
        _config = options.Value;
        _urlService = urlService;
        _httpClient = httpClient;
    }



    public async Task<ServiceResponse<ComicDTO>> GetComic(string key, int maxchapter = -1)
    {
        var data = await _comicReposibility.GetComic(key);
        var comicDTO = new ComicDTO(data);
        if (data != null && _userService.CurrentUser != null)
        {
            comicDTO.IsFollow = await _userService.IsFollowComic(data.ID);
        }
        return ServiceUtilily.GetDataRes<ComicDTO>(comicDTO);
    }



    public async Task<ServiceResponse<List<ComicDTO>>> SearchComicByKeyword(string keyword)
    {
        var result = await _comicReposibility.GetComicByKeyword(keyword);
        return ServiceUtilily.GetDataRes<List<ComicDTO>>(result);

    }


    public async Task<ServiceResponse<ListComicDTO>> GetComics(ComicQueryParams comicQueryParams)
    {
        int page = comicQueryParams.page < 1 ? 1 : comicQueryParams.page;
        int step = comicQueryParams.step < 1 ? 10 : comicQueryParams.step;
        var data = await _comicReposibility.GetComics(page, step, comicQueryParams.genre, comicQueryParams.status, comicQueryParams.sort);

        return ServiceUtilily.GetDataRes<ListComicDTO>(data);
    }
    public async Task<ServiceResponse<ListComicDTO>> GetHotComics(int page = 1, int step = 30)
    {
        page = page < 1 ? 1 : page;
        step = step < 1 ? 30 : step;
        var data = await _comicReposibility.GetHotComics(page, step);
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
    // static async Task<List<PageDTO>?> FetchChapterImage(string comic_slug, string chapter_slug, int chapterid)
    // {
    //     List<PageDTO> urls = new List<PageDTO>();
    //     try
    //     {
    //         chapter_slug = chapter_slug.Replace("chapter", "chuong");
    //         string url = $"https://nettruyenviet.com/truyen-tranh/{comic_slug}/{chapter_slug}";
    //         var request = new HttpRequestMessage();
    //         request.RequestUri = new Uri(url);
    //         request.Method = HttpMethod.Get;

    //         request.Headers.Add("Accept", "*/*");
    //         request.Headers.Add("User-Agent", "Thunder Client (https://www.thunderclient.com)");

    //         var response = await _httpClient.SendAsync(request);
    //         string responseBody = await response.Content.ReadAsStringAsync();

    //         HtmlDocument doc = new HtmlDocument();
    //         doc.LoadHtml(responseBody);
    //         HtmlNodeCollection elements = doc.DocumentNode.SelectNodes("//div[contains(@class, 'page-chapter')]");
    //         if (elements == null) return null;
    //         int i = 0;
    //         Random rnd = new Random();
    //         foreach (HtmlNode element in elements)
    //         {
    //             string imgUrl = element.SelectSingleNode("img").GetAttributeValue("data-src", "");
    //             string data = ServiceUtilily.Base64Encode(imgUrl);
    //             int num = rnd.Next();
    //             PageDTO page = new PageDTO()
    //             {
    //                 URL = $"http://localhost:5080/data/img/{num}.jpg?data={data}",
    //                 PageNumber = ++i,

    //             };
    //             urls.Add(page);
    //             // Add imgUrl to list or perform other tasks
    //         }

    //     }
    //     catch (Exception ex)
    //     {
    //         Console.WriteLine("Error fetching chapter images: " + ex.Message);
    //         return urls;
    //     }
    //     return urls;
    // }
    // public async Task<byte[]> LoadImage(string url)
    // {
    //     HttpRequestMessage? request = new HttpRequestMessage();
    //     request.RequestUri = new Uri(url);
    //     request.Method = HttpMethod.Get;
    //     request.Headers.Add("Accept", "*/*");
    //     // request.Headers.Add("User-Agent", "Mozilla / 5.0(Windows NT 10.0; Win64; x64) AppleWebKit / 537.36(KHTML, like Gecko) Chrome / 130.0.0.0 Safari / 537.36");
    //     request.Headers.Add("Referer", "nettruyenviet.com");
    //     HttpResponseMessage? response = await _httpClient.SendAsync(request);
    //     byte[]? imgByte = await response.Content.ReadAsByteArrayAsync();
    //     return imgByte;
    // }

    public async Task<ServiceResponse<ChapterPageDTO>> GetPagesInChapter(int chapter_id)
    {

        Chapter? chapter = await _comicReposibility.GetChapter(chapter_id);
        if (chapter == null) return ServiceUtilily.GetDataRes<ChapterPageDTO>(null);

        Comic? comic = (await _comicReposibility.GetAllComics())?[chapter.ComicID];

        if (comic == null) return ServiceUtilily.GetDataRes<ChapterPageDTO>(null);

        ComicDTO? comicDTO = new ComicDTO(comic)
        {
            CoverImage = _urlService.GetComicCoverImagePath(comic.CoverImage)
        };
        List<PageDTO>? urlsData = null;
        if (chapter.Pages != null)
        {
            bool needEncode = false;// = chapter.Pages.Contains("s3.mideman.com");
            List<string>? links = JsonSerializer.Deserialize<List<string>>(chapter.Pages.Replace("'", "\""));
            if (links != null)
            {
                urlsData = links.Select((x, i) =>
                {
                    string newUrl = x;
                    if (needEncode)
                    {
                        Uri uri = new Uri(x);
                        string path = uri.AbsolutePath.Replace("nettruyen", "image");
                        newUrl = $"{_urlService.Host}/api{path}?data={ServiceUtilily.Base64Encode(x)}";
                    }
                    return new PageDTO { URL = newUrl, PageNumber = i };
                }).ToList();
            }
        }

        return ServiceUtilily.GetDataRes<ChapterPageDTO>(
            new ChapterPageDTO(chapter)
            {
                Pages = urlsData,
                Comic = comicDTO
            }
        );
    }

    public async Task<ServiceResponse<List<ChapterDTO>>> GetChapters(int comicid)
    {
        var data = await _comicReposibility.GetChapters(comicid);
        return ServiceUtilily.GetDataRes<List<ChapterDTO>>(data);

    }

    // public async Task<ServiceResponse<List<ComicDTO>>> GetSimilarComics(string key)
    // {

    //     ServiceResponse<List<ComicDTO>>? response = await SearchComicByKeyword(key);
    //     foreach (var item in response.Data!)
    //     {
    //         item.Chapters = await _comicReposibility.GetChapters(item.ID.ToString()) ?? [];
    //     }

    //     return response;
    // }
    public async Task<ServiceResponse<List<ComicDTO>>> FindSimilarComics(int id)
    {

        var resultDTO = await _comicReposibility.FindSimilarComics(id);
        return ServiceUtilily.GetDataRes<List<ComicDTO>>(resultDTO);

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
        int page = Math.Max(1, query.Page);
        int step = Math.Max(1, query.Step);
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
    public async Task<ServiceResponse<int>> TotalViewComics(int chapterid)
    {
        var chapter = await _comicReposibility.GetChapter(chapterid);
        if (chapter == null) return ServiceUtilily.GetDataRes<int>(0);
        chapterViews.AddOrUpdate(chapter.ID, 1, (key, oldValue) => oldValue + 1);
        comicViews.AddOrUpdate(chapter.ComicID, 1, (key, oldValue) => oldValue + 1);
        return ServiceUtilily.GetDataRes<int>(chapterViews[chapterid]);
    }

    public async Task UpdateViewChapter()
    {
        if (await _comicReposibility.UpdateViewChapter(chapterViews.ToDictionary()))
        {
            chapterViews.Clear();
        }
    }
    public async Task UpdateViewComic()
    {
        if (await _comicReposibility.UpdateViewComic(comicViews.Keys.ToHashSet()))
        {
            comicViews.Clear();
        }
    }

    public async Task<ServiceResponse<List<ComicDTO>>> GetComicsByIds(List<int> ids)
    {
        List<ComicDTO>? data = await _comicReposibility.GetComicsByIds(ids);
        return ServiceUtilily.GetDataRes<List<ComicDTO>>(data);
    }

    public async Task<ServiceResponse<bool>> ReportError(string name, int chapterid, string errorType, string message)
    {
        var data = new
        {
            name = name,
            errorType = errorType,
            createdAt = DateTime.UtcNow,
            message = message,
        };
        HttpResponseMessage? response = await _httpClient.PostAsJsonAsync(_config.ReportApiUrl, data);
        if (response.IsSuccessStatusCode)
        {
            return ServiceUtilily.GetDataRes<bool>(true);
        }
        return ServiceUtilily.GetDataRes<bool>(false);
    }
}

