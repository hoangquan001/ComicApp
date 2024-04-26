
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
[ApiController]
[Route("")]
public class ComicController : ControllerBase
{
    IComicService _comicService;
    public ComicController(IComicService comicService)
    {
        _comicService = comicService;
    }

    [HttpGet("Comics")]
    public async Task<ActionResult<List<ComicDTO>>> GetComics(SortType sort = SortType.TopAll, ComicStatus status = ComicStatus.All, int genre = -1, int page = 1, int step = 100)
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
    public async Task<ActionResult<ComicDTO>> GetComic(string key)
    {
        ServiceResponse<ComicDTO>? data = await _comicService.GetComic(key);

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

    [HttpGet("Comic/{comic_key}/chapter/{chapter_key}")]
    public async Task<ActionResult<Comic>> GetPagesInChapter(int comic_key, int chapter_key)
    {
        var data = await _comicService.GetPagesInChapter(this.HttpContext.Request.Headers, comic_key, chapter_key);
        if (data.Data == null)
        {
            return NotFound(data);
        }
        return Ok(data);
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
}