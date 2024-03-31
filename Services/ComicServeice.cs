
using ComicApp.Data;
using ComicApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using System.Text;
using System.Linq;
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

    public ServiceResponse<Comics> GetComic(int id)
    {
        return new ServiceResponse<Comics>
        {
            Data = _dbContext.Comics.SingleOrDefault(comic => comic.id == id),
            Status = 0,
            Message = "Success"
        };
    }
    public ServiceResponse<List<Comics>> GetComics(int page, int step)
    {
        if (page < 1) page = 1;
        var xdata = _dbContext.Genres.ToList();
        var data2 = _dbContext.ComicGenre.ToList();
        var data3 = _dbContext.Chapters.FirstOrDefault();
        var data = _dbContext.Comics.Skip((page - 1) * step).Take(step).ToList();
        var datax = _dbContext.Chapters.Where(c => c.comicid == 1).ToList();
        var l = _dbContext.ComicGenre.Where(c => c.comicid == data[0].id).ToList();
        return new ServiceResponse<List<Comics>>
        {
            Data = data,
            Status = 0,
            Message = "Success"
        };
    }

    public ServiceResponse<Comics> AddComic(Comics comic)
    {
        _dbContext.Comics.Add(comic);
        _dbContext.SaveChanges();
        return new ServiceResponse<Comics>
        {
            Data = comic,
            Status = 0,
            Message = "Success"
        };
    }

    public ServiceResponse<List<Genres>> GetGenres()
    {
        _dbContext.Genres.ToList();
        return new ServiceResponse<List<Genres>>
        {
            Data = _dbContext.Genres.ToList(),
            Status = 0,
            Message = "Success"
        };
    }
}