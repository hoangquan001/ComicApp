
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
using ComicAPI.Enums;
using System.Linq.Expressions;
namespace ComicApp.Services;
static class  DbSetExtension
{
    public static IOrderedQueryable<Comic> OrderComicByType(this DbSet<Comic> query, SortType sortType)
    {
        switch (sortType)
        {
            case SortType.Chapter:
                return query.Include(x => x.Chapters).OrderBy(x => x.Chapters.Count);
            case SortType.LastUpdate:
                return query.OrderBy(x => x.CreateAt);
            // case SortType.TopFollow:
            //     return query.OrderBy(x => x.Follow);
            // case SortType.TopComment:
            //     return query.OrderBy(x => x.Comment);
            case SortType.NewComic:
                return query.OrderBy(x => x.Chapters);
            case SortType.TopDay:
                return query.OrderBy(x => x.CreateAt);
            case SortType.TopWeek:
                return query.OrderBy(x => x.CreateAt);
            case SortType.TopMonth:
                query.SelectMany(x => x.Chapters).Where(c => c.UpdateAt > DateTime.Now.AddDays(-30));
                return query.Where(c =>query.Contains(c)).OrderBy(x => x.Chapters.Sum(x=>x.ViewCount));
            case SortType.TopAll:
                return query.OrderBy(x => x.Chapters.Sum(x=>x.ViewCount));
        
        }
        return query.OrderBy(x => x.CreateAt);
    }
}
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


    public async Task<ServiceResponse<List<Comic>>> GetComics(int page, int step, SortType sortType = SortType.TopAll)
 
    {
        if (page < 1) page = 1;
        var data =  _dbContext.Comics.
        OrderComicByType(sortType).
        Skip((page - 1) * step).
        Take(step);
        return new ServiceResponse<List<Comic>>
        {
            Data =  await data.ToListAsync(),
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

    public ServiceResponse<List<Comic>> GetComicsByGenre(int genre, int page, int step)
    {
        var data = _dbContext.Comics.OrderComicByType(SortType.TopAll)
        .Include(x=>x.genres)
        .Where(x=>x.genres
        .Any(g=>g.ID == genre)).
        Skip((page - 1) * step).
        Take(step).ToList();
        return new ServiceResponse<List<Comic>>
        {
            Data = data,
            Status = 0,
            Message = "Success"
        };
    }

    Task<ServiceResponse<List<Comic>>> IComicService.GetComicsByGenre(int genre, int page, int step)
    {
        throw new NotImplementedException();
    }


}