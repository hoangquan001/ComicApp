using ComicAPI.Classes;
using ComicAPI.Enums;
using ComicApp.Models;
using Microsoft.AspNetCore.Mvc;

public interface IComicService
{

    Task<ServiceResponse<List<ComicDTO>>> GetComics(ComicQueryParams comicQueryParams);
    Task<ServiceResponse<ComicDTO>> GetComic(string key);
    Task<ServiceResponse<List<ChapterDTO>>> GetChaptersComic(string key);
    Task<ServiceResponse<ChapterPageDTO>> GetPagesInChapter( int chapter_id);
    Task<ServiceResponse<Comic>> AddComic(Comic comic);
    Task<ServiceResponse<List<Genre>>> GetGenres();

    Task<byte[]> LoadImage(string url);

}