
using ComicApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ComicAPI.DTOs;
[ApiController]
[Route("")]
[Authorize]
public class UserController : ControllerBase
{
    IUserService _userService;
    public UserController(IUserService uerService)
    {
        _userService = uerService;
    }
    [HttpGet("user/me")]
    public async Task<ActionResult<ServiceResponse<UserDTO>>> GetMyUserInfo()
    {
        return Ok(await _userService.GetMyUserInfo());
    }
    //no authorize
    
    [HttpGet("user/{id}")]
    [AllowAnonymous]
    public async Task<ActionResult<ServiceResponse<UserDTO>>> GetUserInfo(int id)
    {
        return Ok(await _userService.GetUserInfo(id));
    }
    [HttpPost("user/follow")]
    public async Task<ActionResult<ServiceResponse<int>>> FollowComic(int comicid, bool follow = true)
    {
        if (follow)
        {
            return Ok(await _userService.FollowComic(comicid));
        }
        return Ok(await _userService.UnFollowComic(comicid));
    }

    [HttpPost("user/comment")]
    public async Task<ActionResult<ServiceResponse<CommentDTO>>> CommentComic(AddCommentDTO addCommentDTO)
    {
        var data = await _userService.AddComment(addCommentDTO.Content!, addCommentDTO.ChapterId, addCommentDTO.ParentCommentId);
        return Ok(data);
    }

    [HttpGet("user/followedComics")]
    public async Task<ActionResult<ServiceResponse<List<ListComicDTO>>>> GetFollowedComics(int page = 1, int size = 40)
    {
        return Ok(await _userService.GetFollowComics(page, size));
    }

    [HttpGet("comments/comic/{comicId}")]
    [AllowAnonymous]
    public async Task<ActionResult<ServiceResponse<CommentPageDTO>>> GetCommentsOfComic(int comicId, int page = 1, int size = 10)
    {
        return Ok(await _userService.GetCommentsOfComic(comicId, page, size));
    }

    [HttpGet("comments/chapter/{chapterId}")]
    [AllowAnonymous]
    public async Task<ActionResult<ServiceResponse<CommentPageDTO>>> GetCommentsOfChapter(int chapterId, int page = 1, int size = 10)
    {
        return Ok(await _userService.GetCommentsOfChapter(chapterId, page, size));
    }
    [HttpPost("user/update")]
    public async Task<ActionResult<ServiceResponse<UserDTO>>> UpdateInfo(UpdateUserInfo request)
    {

        return Ok(await _userService.UpdateInfo(request));
    }
    [HttpPost("user/update/password")]
    public async Task<ActionResult<ServiceResponse<string>>> UpdatePassword(UpdateUserPassword request)
    {

        return Ok(await _userService.UpdatePassword(request));
    }
    [HttpPost("user/update/avatar")]
    public async Task<ActionResult<ServiceResponse<string>>> UpdateAvatar(IFormFile image)
    {

        return Ok(await _userService.UpdateAvatar(image));
    }
    [HttpPost("user/update/typelevel/{typelevel}")]
    public async Task<ActionResult<ServiceResponse<string>>> UpdateTypeLevel(int typelevel)
    {

        return Ok(await _userService.UpdateTypelevel(typelevel));
    }
    [HttpPost("user/update/maxim")]
    public async Task<ActionResult<ServiceResponse<string>>> UpdateMaxim(string? maxim)
    {

        return Ok(await _userService.UpdateMaxim(maxim));
    }


    [HttpGet("user/notify")]
    public async Task<ActionResult<ServiceResponse<List<UserNotificationDTO>>>> GetUserNotify()
    {

        return Ok(await _userService.GetUserNotify());
    }
    [HttpPost("user/notify/update")]
    public async Task<ActionResult<ServiceResponse<string>>> UpdateUserNotify(
       UpdateUserNotifyDTO notify)
    {

        var result = await _userService.UpdateUserNotify(notify.ID, notify.IsRead);
        return Ok(result);
    }
    [HttpDelete("user/notify/delete/{notifyID}")]
    public async Task<ActionResult<ServiceResponse<string>>> DeleteUserNotify(
     int? notifyID)
    {

        var result = await _userService.DeleteUserNotify(notifyID);
        return Ok(result);
    }
    [HttpGet("user/vote")]
    public async Task<ActionResult<ServiceResponse<int>>> GetUserVote(int comicid)
    {
        return Ok(await _userService.GetUserVote(comicid));
    }
    [HttpPost("user/vote/update")]
    public async Task<ActionResult<ServiceResponse<int>>> VoteComic(int comicid, int votePoint)
    {
        return Ok(await _userService.VoteComic(comicid, votePoint));
    }
    [HttpDelete("user/vote/delete")]
    public async Task<ActionResult<ServiceResponse<int>>> UnVoteComic(int comicid)
    {
        return Ok(await _userService.UnVoteComic(comicid));
    }
}