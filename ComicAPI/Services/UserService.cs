
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
    readonly ComicDbContext _dbContext;
    readonly ITokenMgr _tokenMgr;
    //Contructor
    public UserService(ComicDbContext db, ITokenMgr tokenMgr)
    {
        _dbContext = db;
        _tokenMgr = tokenMgr;

    }

    public async Task<ServiceResponse<int>> FollowComic(int userid, int comicid)
    {
        _dbContext.UserFollowComics.Add(new UserFollowComic
        {
            UserID = userid,
            ComicID = comicid
        });
        await _dbContext.SaveChangesAsync();

        return new ServiceResponse<int> { Status = 1, Message = "Success", Data = 1 };
    }

    public Task<ServiceResponse<int>> UnFollowComic(int userid, int comicid)
    {
        throw new NotImplementedException();
    }
}