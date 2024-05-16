
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
public class AuthService : IAuthService
{
    readonly ComicDbContext _dbContext;
    readonly ITokenMgr _tokenMgr;
    //Contructor
    public AuthService(ComicDbContext db, ITokenMgr tokenMgr)
    {
        _dbContext = db;
        _tokenMgr = tokenMgr;

    }

    public async Task<ServiceResponse<UserDTO>> Login(UserLoginDTO userLogin)
    {
        ServiceResponse<UserDTO> res = new ServiceResponse<UserDTO>();
        var data = await _dbContext.Users.SingleOrDefaultAsync(user => user.Email == userLogin.email && user.HashPassword == userLogin.password);
        if (data == null)
        {
            res.Status = 0;
            res.Message = "Username or password is incorrect";
            return res;
        }
        UserDTO user = new UserDTO
        {
            ID = data.ID,
            Username = data.Username,
            Email = data.Email,
            FirstName = data.FirstName,
            LastName = data.LastName,
            Avatar = data.Avatar,
            Gender = data.Gender,
            Token = _tokenMgr.CreateToken(data.ID)
        };
        res.Data = user;
        res.Status = 1;
        res.Message = "Success";
        return res;
    }
    public async Task<ServiceResponse<User>> Register(UserRegisterDTO RegisterData)
    {
        if (await _dbContext.Users.AnyAsync(user => user.Username == RegisterData.username))
        {
            return new ServiceResponse<User> { Status = 0, Message = "Username already exists" };
        }
        User user = new User
        {
            Username = RegisterData.username!,
            Email = RegisterData.email!,
            HashPassword = RegisterData.password!
        };
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();
        return new ServiceResponse<User>
        {
            Data = user,
            Status = 1,
            Message = "Success"
        };
    }
    public async Task<ServiceResponse<string>> Logout(string token)
    {
        _tokenMgr.AddTokenToBlackList(token);
        return new ServiceResponse<string>
        {
            Status = 1,
            Message = "Success"
        };
    }


}