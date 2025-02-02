using ComicAPI.Enums;
using ComicApp.Data;
using ComicApp.Models;

public interface IComicReposibility
{
    public Task<List<ComicDTO>> GetComicsByIds(List<int> ids);
    public Task<ListComicDTO?> GetHotComics(int page, int step);
    public Task<ListComicDTO?> GetComics(int page, int step, int genre = -1, ComicStatus status = ComicStatus.All, SortType sort = SortType.TopAll);
    public Task<Comic?> GetComic(string key);
    public Comic AddComic(Comic comic);
    public List<Genre> GetGenres();
    public byte[] LoadImage(string url);
    public Task<Chapter?> GetChapter(int chapter_id);
    public Task<List<ChapterDTO>?> GetChapters(int comicId);
    public Task<Dictionary<int, Comic>> GetAllComics();
    public Task<List<ComicDTO>> GetComicByKeyword(string keyword);
    public Task<ListComicDTO?> GetComicBySearchAdvance(SortType sort = SortType.TopAll, ComicStatus status = ComicStatus.All,
     List<int>? genres = null, int page = 1, int step = 100, List<int>? Nogenres = null, int? year = null, string? keyword = null);
    public Task<List<ComicDTO>> FindSimilarComics(int comicid);
    public Task<List<ComicDTO>?> GetComicRecommend();
    public Task<ComicTopViewDTO?> GetTopViewComics(int step);
    public Task<bool> SyncViewComic(Dictionary<int, int> comicviews);
    public Task<bool> SyncViewChapter(Dictionary<int, int> chapterviews);
    public Task<List<AnnouncementDTO>?> GetAnnouncements();
}