
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
    [AllowAnonymous]
    public async Task<ActionResult<ServiceResponse<CommentPageDTO>>> GetCommentsOfComic(int comicId, int page = 1, int size = 10)
    {
        return Ok(await _userService.GetCommentsOfComic(comicId, page, size));
    }

    [HttpGet("Comments/chapter/{chapterId}")]
    [AllowAnonymous]
    public async Task<ActionResult<ServiceResponse<CommentPageDTO>>> GetCommentsOfChapter(int chapterId, int page = 1, int size = 10)
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
    [HttpPost("Update/typelevel/{typelevel}")]
    public async Task<ActionResult<ServiceResponse<string>>> UpdateTypeLevel(int typelevel)
    {

        return Ok(await _userService.UpdateTypelevel(typelevel));
    }
    [HttpPost("Update/maxim")]
    public async Task<ActionResult<ServiceResponse<string>>> UpdateMaxim(string? maxim)
    {

        return Ok(await _userService.UpdateMaxim(maxim));
    }
    public async Task<ActionResult<ServiceResponse<string>>> UpdateExp(int exp)
    {

        return Ok(await _userService.UpdateExp(exp));
    }
    [HttpGet("Notify")]
    public async Task<ActionResult<ServiceResponse<List<UserNotificationDTO>>>> GetUserNotify()
    {

        return Ok(await _userService.GetUserNotify());
    }
    [HttpPost("Notify/update")]
    public async Task<ActionResult<ServiceResponse<string>>> UpdateUserNotify(
       UpdateUserNotifyDTO notify)
    {

        var result = await _userService.UpdateUserNotify(notify.ID, notify.IsRead);
        return Ok(result);
    }
    [HttpDelete("Notify/delete/{notifyID}")]
    public async Task<ActionResult<ServiceResponse<string>>> DeleteUserNotify(
     int? notifyID)
    {

        var result = await _userService.DeleteUserNotify(notifyID);
        return Ok(result);
    }
    [HttpGet("Vote")]
    public async Task<ActionResult<ServiceResponse<int>>> GetUserVote(int comicid)
    {
        return Ok(await _userService.GetUserVote(comicid));
    }
    [HttpPost("Vote/update")]
    public async Task<ActionResult<ServiceResponse<int>>> VoteComic(int comicid, int votePoint)
    {
        return Ok(await _userService.VoteComic(comicid, votePoint));
    }
    [HttpDelete("Vote/delete")]
    public async Task<ActionResult<ServiceResponse<int>>> UnVoteComic(int comicid)
    {
        return Ok(await _userService.UnVoteComic(comicid));
    }
}