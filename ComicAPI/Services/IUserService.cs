using ComicAPI.Classes;
using ComicAPI.DTOs;
using ComicAPI.Enums;
using ComicApp.Models;
using Microsoft.AspNetCore.Mvc;

public interface IUserService
{
    public User? CurrentUser { get; set; }
    Task<ServiceResponse<UserDTO>> GetMyUserInfo();
    Task<ServiceResponse<UserDTO>> GetUserInfo(int id);
    Task<ServiceResponse<int>> FollowComic(int comicid);
    Task<ServiceResponse<int>> UnFollowComic(int comicid);
    Task<ServiceResponse<ListComicDTO>> GetFollowComics(int page = 1, int step = 40);
    Task<bool> IsFollowComic(int comicid);
    Task<ServiceResponse<CommentDTO>> AddComment(string content, int chapterid, int? replyFromCmt);
    Task<ServiceResponse<CommentPageDTO>> GetCommentsOfComic(int comicid, int page = 1, int step = 10);
    Task<ServiceResponse<CommentPageDTO>> GetCommentsOfChapter(int chapterid, int page = 1, int step = 10);

    Task<ServiceResponse<UserDTO>> UpdateInfo(UpdateUserInfo request);
    Task<ServiceResponse<string>> UpdatePassword(UpdateUserPassword request);
    Task<ServiceResponse<string>> UpdateAvatar(IFormFile avatar);
    Task<ServiceResponse<string>> UpdateTypelevel(int typelevel);
    Task<ServiceResponse<string>> UpdateMaxim(string? maxim);
    Task<ServiceResponse<List<UserNotificationDTO>>> GetUserNotify();
    Task<ServiceResponse<string>> UpdateUserNotify(int? idNotify = null, bool? isRead = null);
    Task<ServiceResponse<string>> DeleteUserNotify(int? idNotify = null);
    Task<ServiceResponse<int>> GetUserVote(int comicid);
    Task<ServiceResponse<int>> VoteComic(int comicid, int votePoint);
    Task<ServiceResponse<int>> UnVoteComic(int comicid);
    Task<ServiceResponse<int>> TotalExpUser(UserExpType expt = UserExpType.Chapter);
    Task UpdateExp();
}