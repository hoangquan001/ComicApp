using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using ComicAPI.Services;
using ComicApp.Models;
using Microsoft.AspNetCore.Authentication;

using Microsoft.AspNetCore.Http;
namespace ComicAPI.Middleware;

public class TokenHandlerMiddlerware
{
    private readonly RequestDelegate _next;

    private readonly ITokenMgr _tokenMgr;

    public TokenHandlerMiddlerware(RequestDelegate next, ITokenMgr tokenMgr)
    {
        _next = next;
        _tokenMgr = tokenMgr;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {

        if (httpContext.Request.Headers.ContainsKey("Authorization"))
        {
            string token = httpContext.Request.Headers["Authorization"]!;
            token = token.Replace("Bearer ", "");
            if (_tokenMgr.IsTokenBlackList(token))
            {
                httpContext.Request.Headers.Remove("Authorization");
            }
        }
        await _next(httpContext);

    }

   
}
