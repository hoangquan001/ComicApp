using ComicApp.Models;
using Microsoft.AspNetCore.Mvc;

public interface IAuthService
{
    Task<ServiceResponse<string>> Login(UserLoginDTO userLogin);
    Task<ServiceResponse<User>> Register(UserRegisterDTO RegisterData);
    Task<ServiceResponse<string>> Logout();
}