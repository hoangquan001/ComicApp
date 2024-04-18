
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
public class GenreController : ControllerBase
{

    IComicService _comicService;
    //Contructor
    public GenreController(IComicService comicService)
    {
        _comicService = comicService;
    }



    [HttpGet("/{genre}")]
    public async Task<ActionResult<List<Comic>>> GetComicsByGenre(int genre, int page, int step)
    {
        var data = await _comicService.GetComicsByGenre(genre, page, step);
        return Ok(data);
    }


    //Get all Genres
    [HttpGet]
    public ActionResult<List<Genre>> GetGenres()
    {
        var data = _comicService.GetGenres();
        return Ok(data);
    }
}