
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
    readonly DataContext _dbContext;
    //Contructor
    public ComicService(DataContext db)
    {
        _dbContext = db;
    }

    // Get one comic

    public ServiceResponse<Comic> GetComic(int id)
    {
        return new ServiceResponse<Comic>
        {
            Data = _dbContext.Comics.SingleOrDefault(comic => comic.ID == id),
            Status = 0,
            Message = "Success"
        };
    }
    public ServiceResponse<List<Comic>> GetComics(int page, int step)
    {
        if (page < 1) page = 1;
        // var xdata = _dbContext.Genres.ToList();
        // var data2 = _dbContext.ComicGenre.ToList();
        // var data3 = _dbContext.Chapters.FirstOrDefault();
        var data = _dbContext.Comics.
        Include(c => c.genres).
        Include(c => c.Chapters).
        Skip((page - 1) * step).
        Take(step).
        ToList();
        var datax = _dbContext.Chapters
        .Include(c => c.Pages)
        .Where(c => c.ComicID == 1)
        .Select(c => new {c.ComicID} )
        .ToList();
        var pageData = _dbContext.Pages.ToList();
        // var l = _dbContext.ComicGenre.Where(c => c.ComicID == data[0].ID).ToList();
        return new ServiceResponse<List<Comic>>
        {
            Data = data,
            Status = 0,
            Message = "Success"
        };
    }

    public ServiceResponse<Comic> AddComic(Comic comic)
    {
        _dbContext.Comics.Add(comic);
        _dbContext.SaveChanges();
        return new ServiceResponse<Comic>
        {
            Data = comic,
            Status = 0,
            Message = "Success"
        };
    }

    public ServiceResponse<List<Genre>> GetGenres()
    {
        _dbContext.Genres.ToList();
        return new ServiceResponse<List<Genre>>
        {
            Data = _dbContext.Genres.ToList(),
            Status = 0,
            Message = "Success"
        };
    }
}