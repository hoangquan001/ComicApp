
using ComicApp.Models;
using Microsoft.AspNetCore.Mvc;
using ComicAPI.Enums;
using ComicAPI.Classes;
using Microsoft.AspNetCore.RateLimiting;
[ApiController]
[Route("api")]
[EnableRateLimiting("FixedLimiter")]
public class ComicController : ControllerBase
{
    private readonly HttpClient _httpClient;
    IComicService _comicService;
    IUserService _userService;
    public ComicController(IComicService comicService, IUserService userService, HttpClient httpClient)
    {
        _comicService = comicService;
        _userService = userService;
        _httpClient = httpClient;
    }

    [HttpGet("comicsbyids")]
    public async Task<ActionResult<List<ComicDTO>>> GetComics(string ids)
    {
        try
        {
            List<int> list = ids.Split(',').Select(int.Parse).ToList();
            if (list.Count > 40) return BadRequest("List of comic ids is too long");
            return Ok(await _comicService.GetComicsByIds(list));
        }
        catch (System.Exception e)
        {
            return BadRequest(e.Message);
        }
    }
    [HttpGet("comics")]
    public async Task<ActionResult<ListComicDTO>> GetComics(SortType sort = SortType.TopAll, ComicStatus status = ComicStatus.All, int genre = -1, int page = 1, int step = 100, int hot = -1)
    {
        ComicQueryParams queryParams = new ComicQueryParams();
        {
            queryParams.sort = sort;
            queryParams.status = status;
            queryParams.genre = genre;
            queryParams.page = page;
            queryParams.step = step;
            queryParams.hot = hot;
        }
        var data = await _comicService.GetComics(queryParams);
        return Ok(data);
    }
    [HttpGet("hotcomics")]
    public async Task<ActionResult<ListComicDTO>> GetHotComics(int page = 1, int step = 30)
    {
        var data = await _comicService.GetHotComics(page, step);
        return Ok(data);
    }

    //get one comic by id
    // [Authorize]
    [HttpGet("comic/{key}")]
    public async Task<ActionResult<ComicDTO>> GetComic(string key, int mchapter = -1)
    {
        ServiceResponse<ComicDTO>? data = await _comicService.GetComic(key, mchapter);
        if (data.Data == null)
        {
            return NotFound(data);
        }
        return Ok(data);
    }

    [HttpGet("comic/{comic_id}/chapters")]
    public async Task<ActionResult<ComicDTO>> GetChaptersByComic(int comic_id)
    {
        var data = await _comicService.GetChapters(comic_id);

        if (data.Data == null)
        {
            return NotFound(data);
        }
        return Ok(data);
    }

    [HttpGet("comic/chapter/{chapter_key}")]
    public async Task<ActionResult<ChapterPageDTO>> GetPagesInChapter(int chapter_key)
    {
        var data = await _comicService.GetPagesInChapter(chapter_key);
        if (data.Data == null)
        {
            return NotFound(data);
        }
        return Ok(data);
    }

    // [HttpGet("data/img/{img_name}")]
    // public async Task<ActionResult> GetImage(string img_name, [FromQuery]string data)
    // {
        
    //     // HttpContext.Request.Headers;
    //     string url = ServiceUtilily.Base64Decode(data);
    //     byte[]? rawdata = await _comicService.LoadImage(url);

        
    //     return File(rawdata, contentType: "image/jpeg");
    // }

    [HttpGet("image/{*path}")]
    public async Task<ActionResult> HandleImageRequest(string path, [FromQuery] string data)
    {
        // Xử lý logic với URL con trong "path"
        try
        {
            string url = ServiceUtilily.Base64Decode(data);
            
            // byte[]? rawdata = await _comicService.LoadImage(url);
            HttpRequestMessage? request = new HttpRequestMessage();
            request.RequestUri = new Uri(url);
            request.Method = HttpMethod.Get;
            request.Headers.Add("Accept", "*/*");
            request.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 6.2; Win64; x64; en-US) Gecko/20130401 Firefox/53.5");
            request.Headers.Add("Referer", "nettruyenviet.com");
            HttpResponseMessage? response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            if (!response.IsSuccessStatusCode)
            {
                return NotFound(); // Trả về 404 nếu ảnh không tìm thấy
            }
            byte[]? imgByte = await response.Content.ReadAsByteArrayAsync();
            // return imgByte;
            var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/octet-stream";
            return new FileStreamResult(await response.Content.ReadAsStreamAsync(), contentType);
        }
        catch (System.Exception)
        {
            return NotFound("Image not found");
        }
    }

    [HttpGet("comic/search")]

    public async Task<ActionResult<List<ComicDTO>>> SearchComicsByKeyword(string keyword)
    {
        var data = await _comicService.SearchComicByKeyword(keyword);
        return Ok(data);
    }


    [HttpGet("comic/similar/{id}")]
    public async Task<IActionResult> GetSimilarBooks(int id)
    {
        var similarBooks = await _comicService.FindSimilarComics(id);
        return Ok(similarBooks);
    }

    // [HttpPost]
    // public async Task<ActionResult<Comic>> AddComic(Comic comic)
    // {
    //     return Ok();
    // }

    //Get all Genres
    [HttpGet("Genres")]
    public async Task<ActionResult<List<Genre>>> GetGenres()
    {
        var data = await _comicService.GetGenres();
        return Ok(data);
    }
    [HttpGet("comic/advance")]

    public async Task<ActionResult<ListComicDTO>> GetComicBySearchAdvance(
        [FromQuery] SortType sort = SortType.TopAll,
        [FromQuery] ComicStatus status = ComicStatus.All,
        [FromQuery] string? genres = null,
        [FromQuery] int page = 1,
        [FromQuery] int step = 100,
        [FromQuery] string? nogenres = null,
        [FromQuery] int? year = null,
        [FromQuery] string keyword = ""
        )
    {
        ComicQuerySearchAdvance queryParams = new ComicQuerySearchAdvance
        {
            Sort = sort,
            Status = status,
            Genres = genres,
            Page = page,
            Step = step,
            Notgenres = nogenres,
            Year = year,
            Keyword = keyword
        };

        return Ok(await _comicService.GetComicBySearchAdvance(queryParams));
    }
    [HttpGet("comic/recommend")]
    public async Task<ActionResult<List<ComicDTO>>> GetComicRecommend()
    {
        return Ok(await _comicService.GetComicRecommend());
    }
    [HttpGet("comic/topview")]
    public async Task<ActionResult<List<ComicDTO>>> GetTopViewComics()
    {
        return Ok(await _comicService.GetTopViewComics(8));
    }
    [HttpGet("comic/view_exp")]
    public async Task<ActionResult<int>> TotalViewComics(int comicId, int chapterId, UserExpType exp)
    {
        var responseUser = await _userService.TotalExpUser(exp);
        var responseComic = await _comicService.TotalViewComics(chapterId);
        return Ok(responseComic);
    }
    [HttpPost("comic/report")]
    public async Task<ActionResult<bool>> ReportError(ErrorReportDTO data)
    {

        var responseComic = await _comicService.ReportError(data.name, data.chapterid, data.errorType, data.message);
        return Ok(responseComic);
    }
}