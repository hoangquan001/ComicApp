using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using ComicApp.Models;

using Microsoft.AspNetCore.Http;
namespace ComicAPI.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        ServiceResponse<string> res = new ServiceResponse<string>()
        {
            Status = 0,
            Message = exception.Message
        };
        var jsonString = JsonSerializer.Serialize(res);
        await context.Response.WriteAsync(jsonString);
    }
}
