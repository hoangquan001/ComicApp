using ComicAPI.DTOs;
using ComicApp.Models;
using Microsoft.AspNetCore.Mvc;

public interface IAuthService
{
    Task<ServiceResponse<UserDTO>> Login(UserLoginDTO userLogin);
    Task<ServiceResponse<UserDTO>> LoginWithSocial(UserLoginSocialDTO userLogin);
    Task<ServiceResponse<User>> Register(UserRegisterDTO RegisterData);
    Task<ServiceResponse<int>> SendEmailConfirm(int userId, string email);
    Task<ServiceResponse<User>> ConfirmEmail(int userId, string code);
    Task<ServiceResponse<string>> Logout(string token);
    Task<ServiceResponse<string>> ForgotPassword(string email);
}