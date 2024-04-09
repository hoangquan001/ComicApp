using ComicAPI.Enums;
using ComicApp.Models;
using Microsoft.AspNetCore.Mvc;

public interface IComicService
{

    Task<ServiceResponse<List<Comic>>> GetComics(int page, int step, SortType sortType = SortType.TopAll);
    Task<ServiceResponse<List<Comic>>> GetComicsByGenre(int genre, int page, int step);
    Task<ServiceResponse<Comic>> GetComic(int id);
    Task<ServiceResponse<Comic>> AddComic(Comic comic);
    Task<ServiceResponse<List<Genre>>> GetGenres();
    Task<ServiceResponse<List<Comic>>> GetComics(int page, int step);

}