
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
[ApiController]
[Route("[controller]")]
public class ComicController : ControllerBase
{

    IComicService _comicService;
    //Contructor
    public ComicController(IComicService comicService)
    {
        _comicService = comicService;
    }

    [HttpGet]

    public async Task<ActionResult<List<Comic>>> GetComics(int page, int step)
    {
        var data = await _comicService.GetComics(page, step);
        return Ok(data);
    }
    // [HttpGet("Genres/{genre}")]
    // public ActionResult<List<Comic>> GetComicsByGenre(string genre,int page, int step )
    // {
    //     // var data = _comicService.GetComicsByGenre(genre,page, step);
    // }


    //get one comic by id

    [HttpGet("{id}")]
    public ActionResult<Comic> GetComic(int id)
    {
        return Ok();
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