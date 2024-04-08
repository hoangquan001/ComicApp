using ComicApp.Data;
using ComicApp.Models;

class ComicReposibility
{
    private readonly ComicDbContext _dbContext;

    public ComicReposibility(ComicDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Comic AddComic(Comic comic)
    {
        _dbContext.Comics.Add(comic);
        _dbContext.SaveChanges();
        return comic;
    }

    public List<Comic> GetComics(int page, int step)
    {
        return _dbContext.Comics.Skip((page - 1) * step).Take(step).ToList();
    }
}