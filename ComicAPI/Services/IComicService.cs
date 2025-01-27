using ComicAPI.Classes;
using ComicAPI.Enums;
using ComicApp.Models;
using Microsoft.AspNetCore.Mvc;

public interface IComicService
{

    Task<ServiceResponse<ListComicDTO>> GetHotComics(int page = 1, int step = 30);
    Task<ServiceResponse<ListComicDTO>> GetComics(ComicQueryParams comicQueryParams);
    Task<ServiceResponse<List<ComicDTO>>> GetComicsByIds(List<int> ids);
    Task<ServiceResponse<ComicDTO>> GetComic(string key, int chaptertake = -1);
    Task<ServiceResponse<List<ChapterDTO>>> GetChapters(int comicid);
    Task<ServiceResponse<ChapterPageDTO>> GetPagesInChapter(int chapter_id);
    Task<ServiceResponse<Comic>> AddComic(Comic comic);
    Task<ServiceResponse<List<Genre>>> GetGenres();
    Task<ServiceResponse<List<ComicDTO>>> SearchComicByKeyword(string keyword);
    Task<ServiceResponse<List<ComicDTO>>> FindSimilarComics(int id);
    // Task<byte[]> LoadImage(string url);
    Task<ServiceResponse<ListComicDTO>> GetComicBySearchAdvance(ComicQuerySearchAdvance comicQueryParams);
    Task<ServiceResponse<List<ComicDTO>>> GetComicRecommend();
    Task<ServiceResponse<ComicTopViewDTO?>> GetTopViewComics(int step);
    Task<ServiceResponse<int>> CalcView(int chapterid);
    Task<ServiceResponse<bool>> ReportError(string name, int chapterid, string errorType, string? content);
    Task<ServiceResponse<List<AnnouncementDTO>>> GetAnnouncement();
    Task<bool> SyncViewComic();
    Task<bool> SyncViewChapter();

}