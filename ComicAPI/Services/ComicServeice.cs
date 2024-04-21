
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
using AutoMapper;
using ComicAPI.Classes;
namespace ComicApp.Services;

public class ComicService : IComicService
{
    readonly ComicDbContext _dbContext;
    readonly IMapper _mapper;
    //Contructor
    public ComicService(ComicDbContext db, IMapper mapper)
    {
        _dbContext = db;
        _mapper = mapper;
    }

    // Get one comic

    public async Task<ServiceResponse<ComicDTO>> GetComic(int id)
    {

        var data = await _dbContext.Comics.Select(x => new ComicDTO
        {
            ID = x.ID,
            Title = x.Title,
            Author = x.Author,
            URL = x.URL,
            CoverImage = "https://static.doctruyenonline.vn/images/vu-luyen-dien-phong.jpg",
            Description = x.Description,
            Status = x.Status,
            Rating = x.Rating,
            UpdateAt = x.UpdateAt,
            ViewCount = x.Chapters.Sum(x => x.ViewCount),
            genres = x.genres.Select(g => new GenreLiteDTO { ID = g.ID, Title = g.Title }).ToList(),
            Chapters = x.Chapters.OrderByDescending(x => x.ChapterNumber).Select(x => ChapterSelector(x)).ToList()
        })
        .Where(x => x.ID == id)
        .AsSplitQuery()
        .FirstOrDefaultAsync();
        return GetDataRes<ComicDTO>(data);
    }

    public ServiceResponse<T> GetDataRes<T>(T? data)
    {
        var res = new ServiceResponse<T>();

        if (data == null)
        {
            res.Status = 0;
            res.Message = "Not found";

        }
        else
        {
            res.Data = data;
            res.Status = 1;
            res.Message = "Success";
        }

        return res;

    }
    private static ChapterDTO ChapterSelector(Chapter x)
    {
        return new ChapterDTO
        {
            ID = x.ID,
            Title = x.Title,
            ChapterNumber = x.ChapterNumber,
            ViewCount = x.ViewCount,
            UpdateAt = x.UpdateAt
        };
    }

    public async Task<ServiceResponse<List<ComicDTO>>> GetComics(ComicQueryParams comicQueryParams)
    {
        int page = comicQueryParams.page == 0 ? 1 : comicQueryParams.page;
        int step = comicQueryParams.step == 0 ? 10 : comicQueryParams.step;
        var data = await _dbContext.Comics
        .OrderComicByType(comicQueryParams.sort)
        .Select(x =>
        new ComicDTO
        {
            ID = x.ID,
            Title = x.Title,
            Author = x.Author,
            URL = x.URL,
            CoverImage = "https://static.doctruyenonline.vn/images/vu-luyen-dien-phong.jpg",
            Description = x.Description,
            Status = x.Status,
            Rating = x.Rating,
            UpdateAt = x.UpdateAt,
            ViewCount = x.Chapters.Sum(x => x.ViewCount),
            genres = x.genres.Select(g => new GenreLiteDTO { ID = g.ID, Title = g.Title }).ToList(),
            Chapters = x.Chapters.OrderByDescending(x => x.ChapterNumber).Take(3).Select(x => ChapterSelector(x)).ToList()
        })
        .Where(comicQueryParams.status == ComicStatus.All ? x => true : x => x.Status == (int)comicQueryParams.status)
        .Skip((page - 1) * step)
        .Take(step)
        .ToListAsync();
        return new ServiceResponse<List<ComicDTO>>
        {
            Data = data,
            Status = 1,
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
            Status = 1,
            Message = "Success"
        };
    }

    public async Task<ServiceResponse<List<Genre>>> GetGenres()
    {
        // await _dbContext.Genres.ToListAsync();
        return new ServiceResponse<List<Genre>>
        {
            Data = await _dbContext.Genres.ToListAsync(),
            Status = 1,
            Message = "Success"
        };
    }


}

