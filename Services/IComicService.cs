using ComicApp.Models;
using Microsoft.AspNetCore.Mvc;

public interface IComicService
{
    ServiceResponse<List<Comic>> GetComics(int page, int step);
    ServiceResponse<Comic> GetComic(int id);
    ServiceResponse<Comic> AddComic(Comic comic);
    ServiceResponse<List<Genre>> GetGenres();
}