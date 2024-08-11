using ComicAPI.Classes;
using ComicAPI.DTOs;
using ComicAPI.Enums;
using ComicApp.Models;
using Microsoft.AspNetCore.Mvc;

public interface IUserService
{
    public int UserID { get; set; }
    Task<ServiceResponse<UserDTO>> GetMyUserInfo();
    Task<ServiceResponse<int>> FollowComic(int comicid);
    Task<ServiceResponse<int>> FollowComic(int userid, int comicid);
    Task<ServiceResponse<int>> UnFollowComic(int comicid);
    Task<ServiceResponse<int>> UnFollowComic(int userid, int comicid);
    Task<ServiceResponse<ListComicDTO>> GetFollowComics(int page = 1, int step = 40);
    Task<ServiceResponse<ListComicDTO>> GetFollowComics(int userid, int page = 1, int step = 40);
    Task<bool> IsFollowComic(int comicid);
    Task<bool> IsFollowComic(int userid, int comicid);
    Task<ServiceResponse<CommentDTO>> AddComment(int userid, string content, int chapterid, int parentcommentid = 0);
    Task<ServiceResponse<CommentDTO>> AddComment(string content, int chapterid, int parentcommentid = 0);
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
    Task<ServiceResponse<int>> VoteComic(int userid, int comicid, int votePoint);
    Task<ServiceResponse<int>> VoteComic(int comicid, int votePoint);
    Task<ServiceResponse<int>> UnVoteComic(int userid, int comicid);
    Task<ServiceResponse<int>> UnVoteComic(int comicid);

    Task<ServiceResponse<int>> TotalExpUser(UserExpType expt = UserExpType.Chapter);
    Task UpdateExp();
}