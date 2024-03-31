
using ComicApp.Data;
using ComicApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;
using System.Linq;
namespace ComicApp.Services;
public class AuthService : IAuthService
{
    readonly DataContext _dbContext;
    //Contructor
    public AuthService(DataContext db)
    {
        _dbContext = db;
    }

    public ServiceResponse<string> Login(UserLoginDTO userLogin)
    {
        ServiceResponse<string> res = new ServiceResponse<string>();
        var data = _dbContext.Users.SingleOrDefault(user => user.username == userLogin.username && user.hash_password == userLogin.password);
        if (data == null)
        {
            res.Status = 0;
            res.Message = "Tên đăng nhập hoặc mật khẩu không chính xác";
            return res;
        }

        res.Data = CreateToken(data.id);
        res.Status = 1;
        res.Message = "Success";
        return res;
    }
    public ServiceResponse<Users> Register(UserRegisterDTO RegisterData)
    {
        if (_dbContext.Users.Any(user => user.username == RegisterData.username))
        {
            return new ServiceResponse<Users> { Status = 0, Message = "Username already exists" };
        }
        Users user = new Users
        {
            username = RegisterData.username!,
            email = RegisterData.email!,
            hash_password = RegisterData.password!
        };
        _dbContext.Users.Add(user);
        _dbContext.SaveChanges();
        return new ServiceResponse<Users>
        {
            Data = user,
            Status = 1,
            Message = "Success"
        };
    }
    public ServiceResponse<string> Logout()
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