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
    Task<bool> SyncUserExp(Dictionary<int, int> exps);
    Task<int> FollowComic(int userid, int comicid);
    Task<int> UnFollowComic(int userid, int comicid);
    Task<ListComicDTO?> GetFollowComics(int userid, int page = 1, int step = 40);
    Task<bool> IsFollowComic(int userid, int comicid);
    Task<CommentDTO?> AddComment(User user, string content, int chapterid, int? replyFromCmt);
    Task<CommentPageDTO?> GetCommentsOfComic(int comicid, int page = 1, int step = 10);
    Task<User?> UpdateInfo(UpdateUserInfo request);
    Task<User?> UpdatePassword(UpdateUserPassword request);
    Task<User?> UpdateAvatar(int userId, string fileName);
    Task<User?> UpdateTypelevel(int userId, int typelevel);
    Task<User?> UpdateMaxim(int UserID, string? maxim);
    Task<bool> UpdateUserNotify(int userId, int? idNotify = null, bool? isRead = null);
    Task<bool> DeleteUserNotify(int userId, int? idNotify = null);
    Task<bool> VoteComic(int userid, int comicid, int votePoint);
    Task<bool> UnVoteComic(int userid, int comicid);

}
