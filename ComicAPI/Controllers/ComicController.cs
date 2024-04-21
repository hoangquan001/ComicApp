
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
    public async Task<ActionResult<List<ComicDTO>>> GetComics(ComicQueryParams queryParams)
    {
        var data = await _comicService.GetComics(queryParams);
        return Ok(data);
    }

    //get one comic by id
    // [Authorize]
    [HttpGet("Comics/{id}")]
    public async Task<ActionResult<ComicDTO>> GetComic(int id)
    {
        var data = await _comicService.GetComic(id);
        if (data.Data == null)
        {
            return NotFound(data);
        }
        return Ok(data);
    }
    // [HttpGet("Comics/{id}")]
    // public async Task<ActionResult<Comic>> GetComic(int id)
    // {
    //     var data = await _comicService.GetComic(id);
    //     if (data.Data == null)
    //     {
    //         return NotFound(data);
    //     }
    //     return Ok(data);
    // }
    [HttpGet("Comics/{comic_id}/{chapter_id}")]
    public async Task<ActionResult<Comic>> GetPagesInChapter(int comic_id, int chapter_id)
    {
        var data = await _comicService.GetPagesInChapter(this.HttpContext.Request.Headers, comic_id, chapter_id);
        if (data.Data == null)
        {
            return NotFound(data);
        }
        return Ok(data);
    }


    [HttpPost]
    public ActionResult<Comic> AddComic(Comic comic)
    {
        return Ok();
    }

    //Get all Genres
    [HttpGet("Genres")]
    public ActionResult<List<Genre>> GetGenres()
    {
        var data = _comicService.GetGenres();
        return Ok(data);
    }
}