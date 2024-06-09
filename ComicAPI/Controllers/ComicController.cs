
using ComicApp.Data;
using ComicApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;
using System.Linq;
using ComicAPI.Enums;
using Microsoft.AspNetCore.Authorization;
using ComicAPI.Classes;
using ComicApp.Services;
[ApiController]
[Route("")]
public class ComicController : ControllerBase
{
    IComicService _comicService;
    // IUserService _userService;
    public ComicController(IComicService comicService)
    {
        _comicService = comicService;
    }

    [HttpGet("Comics")]
    public async Task<ActionResult<ListComicDTO>> GetComics(SortType sort = SortType.TopAll, ComicStatus status = ComicStatus.All, int genre = -1, int page = 1, int step = 100)
    {
        ComicQueryParams queryParams = new ComicQueryParams();
        {
            queryParams.sort = sort;
            queryParams.status = status;
            queryParams.genre = genre;
            queryParams.page = page;
            queryParams.step = step;
        }
        var data = await _comicService.GetComics(queryParams);
        return Ok(data);
    }

    //get one comic by id
    // [Authorize]
    [HttpGet("Comic/{key}")]
    public async Task<ActionResult<ComicDTO>> GetComic(string key, int mchapter = -1)
    {
        ServiceResponse<ComicDTO>? data = await _comicService.GetComic(key, mchapter);
        if (data.Data == null)
        {
            return NotFound(data);
        }
        return Ok(data);
    }

    [HttpGet("Comic/{key}/chapters")]
    public async Task<ActionResult<ComicDTO>> GetChaptersByComic(string key)
    {
        var data = await _comicService.GetChaptersComic(key);

        if (data.Data == null)
        {
            return NotFound(data);
        }
        return Ok(data);
    }

    [HttpGet("Comic/chapter/{chapter_key}")]
    public async Task<ActionResult<ChapterPageDTO>> GetPagesInChapter(int chapter_key)
    {
        var data = await _comicService.GetPagesInChapter(chapter_key);
        if (data.Data == null)
        {
            return NotFound(data);
        }
        return Ok(data);
    }

    [HttpGet("data/img/{img_name}")]
    public async Task<ActionResult> GetImage(string img_name, string data)
    {
        // HttpContext.Request.Headers;
        string url = ServiceUtilily.Base64Decode(data);
        byte[]? rawdata = await _comicService.LoadImage(url);
        return File(rawdata, contentType: "image/jpeg");
    }
    [HttpGet("api/search")]

    public async Task<ActionResult<List<ComicDTO>>> SearchComicsByKeyword(string keyword)
    {
        var data = await _comicService.SearchComicByKeyword(keyword);
        return Ok(data);
    }

    [HttpGet("Comic/Similar/{key}")]
    public async Task<ActionResult<List<ComicDTO>>> GetSimilarComics(string key)
    {
        var data = await _comicService.GetSimilarComics(key);
        return Ok(data);
    }

    [HttpGet("Comic/{id}/similar")]
    public async Task<IActionResult> GetSimilarBooks(int id)
    {
        var similarBooks = await _comicService.FindSimilarComicsAsync(id);
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
    [HttpGet("Comic/advance")]

    public async Task<ActionResult<ListComicDTO>> GetComicBySearchAdvance(
        [FromQuery] SortType sort = SortType.TopAll,
        [FromQuery] ComicStatus status = ComicStatus.All,
        [FromQuery] string? genres = null,
        [FromQuery] int page = 1,
        [FromQuery] int step = 100,
        [FromQuery] string? Nogenres = null)
    {
        ComicQuerySearchAdvance queryParams = new ComicQuerySearchAdvance();
        {
            queryParams.Sort = sort;
            queryParams.Status = status;
            queryParams.Genres = genres;
            queryParams.Page = page;
            queryParams.Step = step;
            queryParams.Notgenres = Nogenres;
        }

        return Ok(await _comicService.GetComicBySearchAdvance(queryParams));
    }
    [HttpGet("Comic/recommend")]
    public async Task<ActionResult<List<ComicDTO>>> GetComicRecommend()
    {
        return Ok(await _comicService.GetComicRecommend());
    }

}