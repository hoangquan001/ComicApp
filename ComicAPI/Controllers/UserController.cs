
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
    IUserService _userService;
    public UserController(IUserService comicService)
    {
        _userService = comicService;
    }

    [HttpPost("Follow")]
    public async Task<ActionResult<ServiceResponse<int>>> FollowComic(int comicid, bool follow = true)
    {
        if (follow)
        {
            return Ok(await _userService.FollowComic(comicid));
        }
        return Ok(await _userService.UnFollowComic(comicid));
    }
    // [HttpPost("UnFollow")]
    // public async Task<ActionResult<ServiceResponse<int>>> UnFollowComic(int comicid)
    // {
    //     return Ok(await _userService.UnFollowComic(comicid));
    // }

    [HttpGet("FollowedComics")]
    public async Task<ActionResult<ServiceResponse<List<ComicDTO>>>> GetFollowedComics(int page = 1, int size = 40)
    {
        return Ok(await _userService.GetFollowComics(page, size));
    }
}