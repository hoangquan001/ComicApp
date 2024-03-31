using ComicApp.Models;
using Microsoft.AspNetCore.Mvc;

public interface IComicService
{
    ServiceResponse<List<Comics>> GetComics(int page, int step);
    ServiceResponse<Comics> GetComic(int id);
    ServiceResponse<Comics> AddComic(Comics comic);
    ServiceResponse<List<Genres>> GetGenres();
}