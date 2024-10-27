
using ComicApp.Models;
using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;
using ComicAPI.Services;
using ComicAPI.DTOs;
using System.Web;

[ApiController]
[Route("api")]
public class DataController : ControllerBase
{


    //Contructor
    public DataController(IAuthService authService, ITokenMgr tokenMgr)
    {

    }
    [HttpPost]
    [Route("image/upload")]
    public async Task<IActionResult> UploadImage(IFormFile formFile, [FromQuery] string token)
    {
        if (token != "123456")
        {
            return Unauthorized("Invalid token.");
        }
        if (formFile.Length == 0)
            return BadRequest("No file was uploaded");

        string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "CoverImg");
        Directory.CreateDirectory(folderPath);
        string filePath = Path.Combine(folderPath, formFile.FileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await formFile.CopyToAsync(stream);
        }

        return Ok(new { url = $"CoverImg/{formFile.FileName}" });
    }

}

