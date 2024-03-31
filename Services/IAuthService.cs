using ComicApp.Models;
using Microsoft.AspNetCore.Mvc;

public interface IAuthService
{
    ServiceResponse<string> Login(UserLoginDTO userLogin);
    ServiceResponse<Users> Register(UserRegisterDTO RegisterData);
    ServiceResponse<string> Logout();
}