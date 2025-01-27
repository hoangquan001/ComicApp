
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
    private readonly EmailSender _emailSender;

    //Contructor
    public AuthService(ComicDbContext db, ITokenMgr tokenMgr, UrlService urlService, EmailSender emailSender)
    {
        _urlService = urlService;
        _dbContext = db;
        _tokenMgr = tokenMgr;
        _emailSender = emailSender;

    }

    public async Task<ServiceResponse<UserDTO>> Login(UserLoginDTO userLogin)
    {
        ServiceResponse<UserDTO> res = new ServiceResponse<UserDTO>();
        var decoderPassword = ServiceUtilily.Base64Decode(userLogin.password!);
        var data = await _dbContext.Users.SingleOrDefaultAsync(user => user.Email == userLogin.email && (user.HashPassword == userLogin.password || user.HashPassword == decoderPassword));
        if (data == null)
        {
            res.Status = 0;
            res.Message = "Tên đăng nhập hoặc mật khẩu không chính xác.";
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
            Status = data.Status,
            Token = _tokenMgr.CreateToken(data.ID),
            TypeLevel = data.TypeLevel,
            Experience = data.Experience,
            Maxim = data.Maxim
        };
        res.Data = user;
        res.Status = 1;
        res.Message = "Đăng nhập thành công";
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
                Status = 1

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
            Status = user.Status,
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
    public async Task<ServiceResponse<int>> SendEmailConfirm(int userid, string email)
    {
        var data = await _dbContext.Users.SingleOrDefaultAsync(user => user.Email == email && user.ID == userid);
        if (data?.Status == 1)
        {
            return new ServiceResponse<int> { Status = 0, Message = "Tài khoản đã xác thực vui lòng đăng nhập" };
        }
        try
        {
            var code = _tokenMgr.CreateToken(userid);
            var callbackUrl = _urlService.GetConfirmEmailPath(userid, code);
            var time = DateTime.UtcNow.Date;
            await _emailSender.SendEmailAsync(email!
                      , "Confirm your email",
                      $"Please confirm your account by <a href='{callbackUrl}'>clicking here</a>.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error downloading image: {ex.Message}");
            return new ServiceResponse<int> { Status = 0, Message = "Địa chỉ email không hợp lệ." };
        }

        return new ServiceResponse<int> { Data = userid, Status = 1, Message = "Gửi thư thành công" };
    }
    public async Task<ServiceResponse<User>> Register(UserRegisterDTO registerData)
    {
        if (!ServiceUtilily.IsValidEmail(registerData.email))
        {
            return new ServiceResponse<User> { Status = 0, Message = "Địa chỉ email không hợp lệ." };
        }
        if (await _dbContext.Users.AnyAsync(user => user.Email == registerData.email))
        {
            return new ServiceResponse<User> { Status = 0, Message = "Tài khoản đã tồn tại" };
        }
        // int maxID = await _dbContext.Users.MaxAsync(user => user.ID);
        User user = new User
        {
            FirstName = registerData.name!,
            Email = registerData.email!,
            HashPassword = registerData.password!,
            Status = 1
        };

        // var response = await SendEmailConfirm(user.ID, user.Email);
        // if (response.Status == 0)
        // {
        //     return new ServiceResponse<User> { Status = 0, Message = "Not send email confirm" };
        // }
        _dbContext.Users.Add(user);
        await _dbContext.SaveChangesAsync();

        return new ServiceResponse<User> { Data = user, Status = 1, Message = "Đăng ký thành công" };
    }
    public async Task<ServiceResponse<User>> ConfirmEmail(int UserId, string Code)
    {

        var handler = new JwtSecurityTokenHandler();
        JwtSecurityToken jwtToken;
        try
        {
            jwtToken = handler.ReadJwtToken(Code) as JwtSecurityToken;
        }
        catch (Exception)
        {
            return new ServiceResponse<User> { Status = 0, Message = "Invalid token" };
        }

        if (jwtToken == null)
        {
            return new ServiceResponse<User> { Status = 0, Message = "Invalid token" };
        }

        var expClaim = jwtToken.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Exp);

        if (expClaim == null || !long.TryParse(expClaim.Value, out var exp))
        {
            return new ServiceResponse<User> { Status = 0, Message = "Invalid token" };
        }

        var expirationTime = DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime;
        if (DateTime.UtcNow > expirationTime)
        {
            return new ServiceResponse<User> { Status = 0, Message = "Token has expired" };
        }

        var user = await _dbContext.Users.FindAsync(UserId);
        if (user == null || Code == null)
        {
            return new ServiceResponse<User> { Status = 0, Message = "User not found" };
        }
        if (user.Status == 1)
        {
            return new ServiceResponse<User>
            {

                Status = 0,
                Message = "Tài khoản đã được xác thực, xin vui lòng đăng nhập"
            };
        }
        user.Status = 1;
        await _dbContext.SaveChangesAsync();
        return new ServiceResponse<User>
        {
            Data = user,
            Status = 1,
            Message = "Email confirmed successfully"
        };
    }
    public async Task<ServiceResponse<string>> Logout(string token)
    {
        await Task.FromResult(_tokenMgr.AddTokenToBlackList(token));
        return new ServiceResponse<string>
        {
            Status = 1,
            Message = "Success"
        };
    }
    public async Task<ServiceResponse<string>> ForgotPassword(string email)
    {

        var user = await _dbContext.Users.SingleOrDefaultAsync(user => user.Email == email);
        if (user == null)
        {
            return new ServiceResponse<string>
            {
                Status = 0,
                Message = "User not found"
            };
        }

        var encoderPassword = ServiceUtilily.Base64Encode(user.HashPassword!);
        var message = "New password: " + encoderPassword;
        await _emailSender.SendEmailAsync(email, "Reset Password", message);
        return new ServiceResponse<string>
        {
            Status = 1,
            Message = "Success"
        };
    }


}

