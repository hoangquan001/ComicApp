using ComicApp.Models;
using Microsoft.AspNetCore.Mvc;

public interface IAuthService
{
    ServiceResponse<string> Login(UserLoginDTO userLogin);
    ServiceResponse<User> Register(UserRegisterDTO RegisterData);
    ServiceResponse<string> Logout();
}