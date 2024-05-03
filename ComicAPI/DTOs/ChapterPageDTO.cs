using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ComicApp.Data;


public class ChapterPageDTO : ChapterDTO
{
    public List<PageDTO>? Pages { get; set; }
    public ComicDTO? Comic { get; set; }
}

