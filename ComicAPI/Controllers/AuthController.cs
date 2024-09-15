
using ComicApp.Models;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;
using ComicAPI.Services;
using ComicAPI.DTOs;
using System.Web;

[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    readonly IAuthService _authService;

    readonly ITokenMgr _tokenMgr;
    //Contructor
    public AuthController(IAuthService authService, ITokenMgr tokenMgr)
    {
        _authService = authService;
        _tokenMgr = tokenMgr;
    }
    [HttpGet]
    [Route("login")]
    public async Task<ActionResult<ServiceResponse<UserDTO>>> Login(string email, string password)
    {
        UserLoginDTO userLoginDTO = new UserLoginDTO
        {
            email = email,
            password = password
        };
        ServiceResponse<UserDTO> res = await _authService.Login(userLoginDTO);
        return Ok(res);
    }
    [HttpPost]
    [Route("loginWithSocial")]
    public async Task<ActionResult<ServiceResponse<UserDTO>>> LoginWithSocial(UserLoginSocialDTO userLoginDTO)
    {
        return Ok(await _authService.LoginWithSocial(userLoginDTO));
    }

    [Route("register")]
    [HttpPost]
    public async Task<ActionResult<ServiceResponse<User>>> Register(UserRegisterDTO RegisterData)
    {
        ServiceResponse<User> res = await _authService.Register(RegisterData);
        return Ok(res);
    }

    [Route("sendEmailConfirm")]
    [HttpGet]
    public async Task<ActionResult<ServiceResponse<int>>> SendEmailConfirm(int UserId, string email)
    {
        ServiceResponse<int> res = await _authService.SendEmailConfirm(UserId, email);
        return Ok(res);
    }
    [Route("confirmEmail")]
    [HttpGet]
    public async Task<ActionResult<ServiceResponse<string>>> ConfirmEmail(int UserId, string Code)
    {

        ServiceResponse<User> res = await _authService.ConfirmEmail(UserId, Code);
        if (res.Status == 1)
        {
            return Redirect("http://localhost:4200/auth/login");
        }
        else
        {
            string encodedMessage = HttpUtility.UrlEncode(res.Message);
            return Redirect($"http://localhost:4200/auth/confirm-email?mssg={encodedMessage}");
        }
    }
    [Route("logout")]
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<ServiceResponse<string>>> Logout()
    {
        string jwt = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        ServiceResponse<string> res = await _authService.Logout(jwt);
        return Ok(res);
    }
    [Route("forgot")]
    [HttpGet]
    public async Task<ActionResult<ServiceResponse<string>>> ForgotPassword(string email)
    {
        ServiceResponse<string> res = await _authService.ForgotPassword(email);
        return Ok(res);
    }

}

