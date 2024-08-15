using ComicAPI.DTOs;
using ComicAPI.Enums;
using ComicAPI.Models;
using ComicApp.Data;
using ComicApp.Models;

public interface IUserReposibility
{
    Task<User?> GetUser(int userid);
    Task<List<UserNotificationDTO>?> GetUserNotify(int userid);
    Task<UserVoteComic?> GetUserVoteComic(int userid, int comicid);
    Task UpdateUserExp(Dictionary<int, int> exps);
    Task<int> FollowComic(int userid, int comicid);
    Task<int> UnFollowComic(int userid, int comicid);
    Task<ListComicDTO> GetFollowComics(int userid, int page = 1, int step = 40);
    Task<bool> IsFollowComic(int userid, int comicid);
    Task<CommentDTO> AddComment(int userid, string content, int chapterid, int parentcommentid = 0);
    Task<CommentPageDTO> GetCommentsOfComic(int comicid, int page = 1, int step = 10);
    Task<User> UpdateInfo(UpdateUserInfo request);
    Task<string> UpdatePassword(UpdateUserPassword request);
    Task<string> UpdateAvatar(int userId, string fileName);
    Task<string> UpdateTypelevel(int userId,int typelevel);
    Task<string> UpdateMaxim(int UserID, string? maxim);
    Task<bool> UpdateUserNotify(int userId, int? idNotify = null, bool? isRead = null);
    Task<bool> DeleteUserNotify(int userId, int? idNotify = null);
    Task<bool> VoteComic(int userid, int comicid, int votePoint);
    Task<bool> UnVoteComic(int userid, int comicid);

}
