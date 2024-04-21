using ComicAPI.Classes;
using ComicAPI.Enums;
using ComicApp.Models;
using Microsoft.AspNetCore.Mvc;

public interface IComicService
{

    Task<ServiceResponse<List<ComicDTO>>> GetComics(ComicQueryParams comicQueryParams);
    Task<ServiceResponse<ComicDTO>> GetComic(int id);
    Task<ServiceResponse<Comic>> AddComic(Comic comic);
    Task<ServiceResponse<List<Genre>>> GetGenres();

}