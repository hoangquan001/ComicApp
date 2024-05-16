
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
    public async Task<ActionResult<ServiceResponse<int>>> FollowComic(string comickey)
    {
        return Ok(await _comicService.FollowComic(comickey));
    }
}