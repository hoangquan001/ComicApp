
using ComicApp.Data;
using ComicApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;
using System.Linq;
[ApiController]
[Route("[controller]")]
public class AuthController : ControllerBase
{
    readonly IAuthService _authService;
    //Contructor
    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }
    [HttpPost]
    [Route("Login")]
    public ActionResult<ServiceResponse<string>> Login(UserLoginDTO userLogin)
    {
        ServiceResponse<string> res = _authService.Login(userLogin);
        return Ok(res);
    }
    [Route("Register")]
    [HttpPost]
    public ActionResult<ServiceResponse<User>> Register(UserRegisterDTO RegisterData)
    {
        ServiceResponse<User> res = _authService.Register(RegisterData);
        return Ok(res);
    }
    [Route("Logout")]
    [HttpGet]
    public ActionResult<ServiceResponse<string>> Logout()
    {
        ServiceResponse<string> res = _authService.Logout();
        return Ok(res);
    }

}