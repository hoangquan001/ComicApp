
using ComicApp.Data;
using ComicApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using ComicAPI.Services;

namespace ComicApp.Services;
public class UserService : IUserService
{
    readonly IDataService _dataService;
    readonly IComicReposibility _comicReposibility;
    ComicDbContext _dbContext;
    readonly ITokenMgr _tokenMgr;
    private int _UserID = -1;

    public int UserID
    {
        get
        {
            return _UserID;
        }
        set
        {
            _UserID = value;
        }
    }

    //Contructor
    public UserService(ComicDbContext db, ITokenMgr tokenMgr, IComicReposibility comicReposibility, IDataService dataService)
    {
        _comicReposibility = comicReposibility;
        _dataService = dataService;
        _dbContext = db;
        _tokenMgr = tokenMgr;

    }

    public async Task<ServiceResponse<int>> FollowComic(int userid, int comicid)
    {
        if (!_dbContext.Comics.Any(x => x.ID == comicid)) return new ServiceResponse<int> { Status = 0, Message = "Comic not found", Data = 0 };

        var user = await _dbContext.UserFollowComics.FirstOrDefaultAsync(x => x.UserID == userid && x.ComicID == comicid);
        if (user != null) return new ServiceResponse<int> { Status = 0, Message = "Already followed", Data = 0 };
        //check comicid exist


        _dbContext.UserFollowComics.Add(new UserFollowComic { UserID = userid, ComicID = comicid });
        await _dbContext.SaveChangesAsync();

        return new ServiceResponse<int> { Status = 1, Message = "Success", Data = 1 };
    }
    public async Task<ServiceResponse<int>> FollowComic(int comicid)
    {
        return await FollowComic(UserID, comicid);
    }

    public async Task<ServiceResponse<int>> UnFollowComic(int userid, int comicid)
    {
        if (!_dbContext.Comics.Any(x => x.ID == comicid)) return new ServiceResponse<int> { Status = 0, Message = "Comic not found", Data = 0 };

        var user = _dbContext.UserFollowComics.FirstOrDefault(x => x.UserID == userid && x.ComicID == comicid);
        if (user == null) return new ServiceResponse<int> { Status = 0, Message = "Not followed", Data = 0 };
        _dbContext.UserFollowComics.Remove(user);

        await _dbContext.SaveChangesAsync();
        return new ServiceResponse<int> { Status = 1, Message = "Success", Data = 0 };
    }
    public async Task<ServiceResponse<int>> UnFollowComic(int comicid)
    {
        return await UnFollowComic(UserID, comicid);
    }

    public async Task<ServiceResponse<List<ComicDTO>>> GetFollowComics(int userid, int page = 1, int step = 40)
    {
        List<ComicDTO>? data = await _comicReposibility.GetFollowComicsByUser(userid, page, step);
        return ServiceUtilily.GetDataRes(data);
    }
    public async Task<ServiceResponse<List<ComicDTO>>> GetFollowComics(int page = 1, int step = 40)
    {
        return await GetFollowComics(UserID, page, step);
    }

    public async Task<bool> IsFollowComic(int userid, int comicid)
    {
        //check comicid exist

        var user = await _dbContext.UserFollowComics.FirstOrDefaultAsync(x => x.UserID == userid && x.ComicID == comicid);
        return user != null;

    }
    public async Task<bool> IsFollowComic(int comicid)
    {
        return await IsFollowComic(UserID, comicid);
    }
}