using ComicAPI.Classes;
using ComicAPI.Enums;
using ComicApp.Models;
using Microsoft.AspNetCore.Mvc;

public interface IUserService
{
    Task<ServiceResponse<int>> FollowComic(int userid, int comicid);
    Task<ServiceResponse<int>> UnFollowComic(int userid, int comicid);
    Task<ServiceResponse<List<ComicDTO>>> GetFollowComics(int userid, int page = 1, int step = 40);
}