using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ComicApp.Data;
using ComicApp.Models;


public class ChapterPageDTO : ChapterDTO
{
    public ChapterPageDTO(Chapter chapter) : base(chapter)
    {

    }
    public List<PageDTO>? Pages { get; set; }
    public ComicDTO? Comic { get; set; }
}

