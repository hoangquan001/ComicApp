using ComicAPI.Enums;
using ComicApp.Data;
using ComicApp.Models;

public interface IComicReposibility
{

    public Task<ListComicDTO?> GetComics(int page, int step, int genre = -1, ComicStatus status = ComicStatus.All, SortType sort = SortType.TopAll);

    public Task<ComicDTO?> GetComic(string key);
    public Comic AddComic(Comic comic);

    public List<Genre> GetGenres();

    public byte[] LoadImage(string url);
    public Task<Chapter?> GetChapter(int chapter_id);

    public Task<List<ChapterDTO>?> GetChaptersComic(string key);

    public Task<List<ComicDTO>?> SearchComicsByKeyword(string keyword);

    public Task<List<ComicDTO>> GetAllComics();
    public Task<List<ComicDTO>> GetFollowComicsByUser(int userid, int page, int size);
    public Task<ListComicDTO?> GetComicBySearchAdvance(SortType sort = SortType.TopAll, ComicStatus status = ComicStatus.All,
     List<int>? genres = null, int page = 1, int step = 100, List<int>? Nogenres = null);
}