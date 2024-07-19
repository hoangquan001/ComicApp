
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
using ComicAPI.DTOs;
namespace ComicApp.Services;
public class AuthService : IAuthService
{
    private readonly ComicDbContext _dbContext;
    private readonly UrlService _urlService;
    private readonly ITokenMgr _tokenMgr;
    //Contructor
    public AuthService(ComicDbContext db, ITokenMgr tokenMgr, UrlService urlService)
    {
        _urlService = urlService;
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
            // Username = data.Username,
            Email = data.Email,
            FirstName = data.FirstName,
            LastName = data.LastName,
            Avatar = _urlService.GetUserImagePath(data.Avatar),
            Gender = data.Gender,
            Token = _tokenMgr.CreateToken(data.ID),
            TypeLevel = data.TypeLevel,
            Experience = data.Experience,
            Maxim = data.Maxim
        };
        res.Data = user;
        res.Status = 1;
        res.Message = "Success";
        return res;
    }
    public async Task<ServiceResponse<UserDTO>> LoginWithSocial(UserLoginSocialDTO userLogin)
    {
        ServiceResponse<UserDTO> res = new ServiceResponse<UserDTO>();
        var user = await _dbContext.Users.SingleOrDefaultAsync(u => u.Email == userLogin.Email);

        if (user == null)
        {
            var uploadsFolder = _urlService.GetPathSaveUserImage();
            string avatarPath = $"{userLogin.Email}.png";
            string fileavatarPath = Path.Combine(uploadsFolder, avatarPath);
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    byte[] imageBytes = await client.GetByteArrayAsync(userLogin.PhotoUrl);
                    await File.WriteAllBytesAsync(fileavatarPath, imageBytes);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error downloading image: {ex.Message}");
                res.Status = 0;
                res.Message = "Failed to download user avatar.";
                return res;
            }

            user = new User
            {
                FirstName = userLogin.FirstName!,
                LastName = userLogin.LastName!,
                Email = userLogin.Email!,
                Avatar = avatarPath,
                Gender = 0,
                HashPassword = "",

            };
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();


        }
        // // Fetch the newly added user
        user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == userLogin.Email);

        var userDto = new UserDTO
        {
            ID = user!.ID,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Avatar = _urlService.GetUserImagePath(user.Avatar),
            Gender = user.Gender,
            Token = _tokenMgr.CreateToken(user.ID),
            TypeLevel = user.TypeLevel,
            Experience = user.Experience,
            Maxim = user.Maxim
        };

        res.Data = userDto;
        res.Status = 1;
        res.Message = "Success";
        return res;
    }
    public async Task<ServiceResponse<User>> Register(UserRegisterDTO RegisterData)
    {
        if (await _dbContext.Users.AnyAsync(user => user.Email == RegisterData.email))
        {
            return new ServiceResponse<User> { Status = 0, Message = "Username already exists" };
        }
        User user = new User
        {
            FirstName = RegisterData.name!,
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
        await Task.Delay(1000);

        return new ServiceResponse<string>
        {
            Status = 1,
            Message = "Success"
        };
    }


}
