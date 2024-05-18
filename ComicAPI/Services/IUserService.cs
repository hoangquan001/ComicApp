using ComicAPI.Classes;
using ComicAPI.Enums;
using ComicApp.Models;
using Microsoft.AspNetCore.Mvc;

public interface IUserService
{
    public int UserID { get; set; }
    Task<ServiceResponse<int>> FollowComic(int comicid);
    Task<ServiceResponse<int>> FollowComic(int userid, int comicid);
    Task<ServiceResponse<int>> UnFollowComic(int comicid);
    Task<ServiceResponse<int>> UnFollowComic(int userid, int comicid);
    Task<ServiceResponse<List<ComicDTO>>> GetFollowComics(int page = 1, int step = 40);
    Task<ServiceResponse<List<ComicDTO>>> GetFollowComics(int userid, int page = 1, int step = 40);
    Task<bool> IsFollowComic(int comicid);
    Task<bool> IsFollowComic(int userid, int comicid);
}