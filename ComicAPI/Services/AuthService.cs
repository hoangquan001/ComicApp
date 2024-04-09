
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
namespace ComicApp.Services;
public class AuthService : IAuthService
{
    readonly ComicDbContext _dbContext;
    //Contructor
    public AuthService(ComicDbContext db)
    {
        _dbContext = db;
    }

    public async Task<ServiceResponse<string>> Login(UserLoginDTO userLogin)
    {
        ServiceResponse<string> res = new ServiceResponse<string>();
        var data = await _dbContext.Users.SingleOrDefaultAsync(user => user.Username == userLogin.username && user.HashPassword == userLogin.password);
        if (data == null)
        {
            res.Status = 0;
            res.Message = "Tên đăng nhập hoặc mật khẩu không chính xác";
            return res;
        }

        res.Data = CreateToken(data.ID);
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
    public async Task<ServiceResponse<string>> Logout()
    {
        return new ServiceResponse<string>
        {
            Status = 1,
            Message = "Success"
        };
    }

    string CreateToken(int user_id)
    {
        //TODO: Implement Realistic Implementation
        var tokenHandler = new JwtSecurityTokenHandler();
        var keyBytes = Encoding.UTF8.GetBytes("hoang anh 34534534534534543quan 123456789");

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, $"{user_id}"),
        };
        // Expires = DateTime.UtcNow.AddDays(7),
        SigningCredentials cred = new SigningCredentials(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature);
        JwtSecurityToken tokenjwt = new
        (
            claims: claims,
            expires: DateTime.UtcNow.AddDays(1),
            signingCredentials: cred
        );
        return tokenHandler.WriteToken(tokenjwt);
    }
}