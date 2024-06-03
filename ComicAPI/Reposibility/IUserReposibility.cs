using ComicAPI.Enums;
using ComicApp.Data;
using ComicApp.Models;

public interface IUserReposibility
{
    Task<User?> GetUser(int userid);

}