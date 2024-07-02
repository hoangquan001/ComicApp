
using System.ComponentModel.DataAnnotations;

public class ComicTopViewDTO
{
    public List<ComicDTO>? DailyComics { get; set; }
    public List<ComicDTO>? WeeklyComics { get; set; }
    public List<ComicDTO>? MonthlyComics { get; set; }
}