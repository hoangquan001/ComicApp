using ComicAPI.DTOs;
using ComicAPI.Enums;
using ComicAPI.Models;
using ComicApp.Data;
using ComicApp.Models;

public interface IUserReposibility
{
    Task<User?> GetUser(int userid);
    Task<List<UserNotificationDTO>?> GetUserNotify(int userid);
}