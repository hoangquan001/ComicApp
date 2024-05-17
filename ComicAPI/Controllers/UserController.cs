
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
[Route("[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    IUserService _comicService;
    public UserController(IUserService comicService)
    {
        _comicService = comicService;
    }

    [HttpPost("Follow")]
    public async Task<ActionResult<ServiceResponse<int>>> FollowComic(int comicid)
    {
        string userid = this.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        return Ok(await _comicService.FollowComic(int.Parse(userid), comicid));
    }
    [HttpPost("UnFollow")]
    public async Task<ActionResult<ServiceResponse<int>>> UnFollowComic(int comicid)
    {
        string userid = this.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        return Ok(await _comicService.UnFollowComic(int.Parse(userid), comicid));
    }

    [HttpGet("FollowedComics")]
    public async Task<ActionResult<ServiceResponse<List<ComicDTO>>>> GetFollowedComics(int page = 1, int size = 40)
    {
        string userid = this.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value;
        return Ok(await _comicService.GetFollowComics(int.Parse(userid), page, size));
    }
}