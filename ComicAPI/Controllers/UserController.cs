
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
using ComicAPI.DTOs;
[ApiController]
[Route("[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    IUserService _userService;
    public UserController(IUserService uerService)
    {
        _userService = uerService;
    }
    [HttpGet]
    public async Task<ActionResult<ServiceResponse<UserDTO>>> GetMyUserInfo()
    {
        return Ok(await _userService.GetMyUserInfo());


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

    [HttpPost("Comment")]
    public async Task<ActionResult<ServiceResponse<CommentDTO>>> CommentComic(AddCommentDTO addCommentDTO)
    {
        var data = await _userService.AddComment(addCommentDTO.Content!, addCommentDTO.ChapterId, addCommentDTO.ParentCommentId);
        return Ok(data);
    }

    [HttpGet("FollowedComics")]
    public async Task<ActionResult<ServiceResponse<List<ListComicDTO>>>> GetFollowedComics(int page = 1, int size = 40)
    {
        return Ok(await _userService.GetFollowComics(page, size));
    }

    [HttpGet("Comments/comic/{comicId}")]
    public async Task<ActionResult<ServiceResponse<List<CommentDTO>>>> GetCommentsOfComic(int comicId, int page = 1, int size = 40)
    {
        return Ok(await _userService.GetCommentsOfComic(comicId, page, size));
    }
    [HttpGet("Comments/chapter/{chapterId}")]
    public async Task<ActionResult<ServiceResponse<List<CommentDTO>>>> GetCommentsOfChapter(int chapterId, int page = 1, int size = 40)
    {
        return Ok(await _userService.GetCommentsOfChapter(chapterId, page, size));
    }
    [HttpPost("Update")]
    public async Task<ActionResult<ServiceResponse<UserDTO>>> UpdateInfo(UpdateUserInfo request)
    {

        return Ok(await _userService.UpdateInfo(request));
    }
    [HttpPost("Update/password")]
    public async Task<ActionResult<ServiceResponse<string>>> UpdatePassword(UpdateUserPassword request)
    {

        return Ok(await _userService.UpdatePassword(request));
    }
    [HttpPost("Update/avatar")]
    public async Task<ActionResult<ServiceResponse<string>>> UpdateAvatar(IFormFile image)
    {

        return Ok(await _userService.UpdateAvatar(image));
    }
    [HttpGet("Notify")]
    public async Task<ActionResult<ServiceResponse<List<UserNotificationDTO>>>> GetUserNotify()
    {

        return Ok(await _userService.GetUserNotify());
    }
}