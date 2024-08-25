using ComicAPI.Enums;
using ComicApp.Data;
using ComicApp.Models;

public interface IComicReposibility
{
    public Task<List<ComicDTO>> GetComicsByIds(List<int> ids);
    public Task<ListComicDTO?> GetComics(int page, int step, int hot, int genre = -1, ComicStatus status = ComicStatus.All, SortType sort = SortType.TopAll);
    public Task<ComicDTO?> GetComic(string key);
    public Comic AddComic(Comic comic);
    public List<Genre> GetGenres();
    public byte[] LoadImage(string url);
    public Task<Chapter?> GetChapter(int chapter_id);
    public Task<List<ChapterDTO>?> GetChaptersComic(string key);
    public Task<List<ComicDTO>> GetAllComics();
    public Task<List<ComicDTO>> GetComicByKeyword(string keyword);
    public Task<ListComicDTO?> GetComicBySearchAdvance(SortType sort = SortType.TopAll, ComicStatus status = ComicStatus.All,
     List<int>? genres = null, int page = 1, int step = 100, List<int>? Nogenres = null, int? year = null, string? keyword = null);
    public Task<List<ComicDTO>?> GetComicRecommend();
    public Task<ComicTopViewDTO?> GetTopViewComics(int step);
    public Task UpdateViewComic(HashSet<int> comicviews);
    public Task UpdateViewChapter(Dictionary<int, int> chapterviews);
}