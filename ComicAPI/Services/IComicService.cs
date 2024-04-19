using ComicAPI.Enums;
using ComicApp.Models;
using Microsoft.AspNetCore.Mvc;

public interface IComicService
{

    Task<ServiceResponse<List<ComicDTO>>> GetComics(int page, int step, SortType sortType = SortType.TopAll);
    Task<ServiceResponse<List<Comic>>> GetComicsByGenre(int genre, int page, int step);
    Task<ServiceResponse<ComicDTO>> GetComic(int id);
    Task<ServiceResponse<ChapterPageDTO>> GetPagesInChapter(IHeaderDictionary headers,int comic_id, int chapter_id);
    Task<ServiceResponse<Comic>> AddComic(Comic comic);
    Task<ServiceResponse<List<Genre>>> GetGenres();

}