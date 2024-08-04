
using ComicApp.Data;
using ComicApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using ComicAPI.Services;
using ComicAPI.DTOs;

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
    [Route("Login")]
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
    [Route("LoginWithSocial")]
    public async Task<ActionResult<ServiceResponse<UserDTO>>> LoginWithSocial(UserLoginSocialDTO userLoginDTO)
    {
        return Ok(await _authService.LoginWithSocial(userLoginDTO));
    }

    [Route("Register")]
    [HttpPost]
    public async Task<ActionResult<ServiceResponse<User>>> Register(UserRegisterDTO RegisterData)
    {
        ServiceResponse<User> res = await _authService.Register(RegisterData);
        return Ok(res);
    }

    [Route("SendEmailConfirm")]
    [HttpGet]
    public async Task<ActionResult<ServiceResponse<int>>> SendEmailConfirm(int UserId, string email)
    {
        ServiceResponse<int> res = await _authService.SendEmailConfirm(UserId, email);
        return Ok(res);
    }
    [Route("ConfirmEmail")]
    [HttpGet]
    public async Task<ActionResult<ServiceResponse<string>>> ConfirmEmail(int UserId, string Code, string Date)
    {

        ServiceResponse<User> res = await _authService.ConfirmEmail(UserId, Code, Date);
        if (res.Status == 1)
        {
            return Redirect("http://localhost:4200/auth/login");
        }

        return Ok(res);
    }
    [Route("Logout")]
    [HttpGet]
    [Authorize]
    public async Task<ActionResult<ServiceResponse<string>>> Logout()
    {
        string jwt = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
        ServiceResponse<string> res = await _authService.Logout(jwt);
        return Ok(res);
    }

}

