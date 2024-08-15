
using System.Security.Claims;
using ComicAPI.Reposibility;
using ComicApp.Models;
using ComicApp.Services;

public class UserMiddleware
{
    private readonly RequestDelegate _next;

    public UserMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IUserService userService, IUserReposibility userReposibility)
    {
        if (context.User?.Identity?.IsAuthenticated ?? false)
        {
            if (userService != null)
            {
                var strId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                userService.CurrentUser = await userReposibility.GetUser(int.Parse(strId!));
            }
        }
        await _next(context);
    }


}
