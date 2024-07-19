using ComicAPI.DTOs;
using ComicApp.Models;
using Microsoft.AspNetCore.Mvc;

public interface IAuthService
{
    Task<ServiceResponse<UserDTO>> Login(UserLoginDTO userLogin);
    Task<ServiceResponse<UserDTO>> LoginWithSocial(UserLoginSocialDTO userLogin);
    Task<ServiceResponse<User>> Register(UserRegisterDTO RegisterData);
    Task<ServiceResponse<string>> Logout(string token);
}