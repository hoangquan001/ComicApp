
using ComicApp.Data;
using ComicApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;
using System.Linq;
using Microsoft.EntityFrameworkCore;
namespace ComicApp.Services;
public class ComicService : IComicService
{
    readonly ComicDbContext _dbContext;
    //Contructor
    public ComicService(ComicDbContext db)
    {
        _dbContext = db;
    }

    // Get one comic

    public async Task<ServiceResponse<Comic>> GetComic(int id)
    {
        return new ServiceResponse<Comic>
        {
            Data = await _dbContext.Comics.SingleOrDefaultAsync(comic => comic.ID == id),
            Status = 0,
            Message = "Success"
        };
    }
    public async Task<ServiceResponse<List<Comic>>> GetComics(int page, int step)
    {
        if (page < 1) page = 1;
        var xdata = _dbContext.Genres.ToList();
        // var data2 = _dbContext.ComicGenre.ToList();
        // var data3 = _dbContext.Chapters.FirstOrDefault();
        var data = _dbContext.Comics.
        Include(c => c.genres).
        // Include(c => c.Chapters).
        OrderBy(c => c.CreateAt).
        Skip((page - 1) * step).
        Take(step)?.
        ToList();
        // var datax = _dbContext.Chapters
        // .Include(c => c.Pages)
        // .Where(c => c.ComicID == 1)
        // .Select(c => new {c.ComicID} )
        // .ToList();
        var a = new { a = 1 };
        var pageData = await _dbContext.Pages.ToListAsync();
        // var l = _dbContext.ComicGenre.Where(c => c.ComicID == data[0].ID).ToList();
        return new ServiceResponse<List<Comic>>
        {
            Data = data,
            Status = 0,
            Message = "Success"
        };
    }

    public async Task<ServiceResponse<Comic>> AddComic(Comic comic)
    {
        _dbContext.Comics.Add(comic);
        await _dbContext.SaveChangesAsync();
        return new ServiceResponse<Comic>
        {
            Data = comic,
            Status = 0,
            Message = "Success"
        };
    }

    public async Task<ServiceResponse<List<Genre>>> GetGenres()
    {
        // await _dbContext.Genres.ToListAsync();
        return new ServiceResponse<List<Genre>>
        {
            Data = await _dbContext.Genres.ToListAsync(),
            Status = 0,
            Message = "Success"
        };
    }
}