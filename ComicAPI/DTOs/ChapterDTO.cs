using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ComicApp.Data;
using ComicApp.Models;


public class ChapterDTO
{
    public ChapterDTO(Chapter chapter)
    {
        ID = chapter.ID;
        Title = chapter.Title;
        Slug = chapter.Url;
        ViewCount = chapter.ViewCount;
        UpdateAt = chapter.UpdateAt;
    }
    public int ID { get; set; } // Primary Key (implicitly set by IDENTITY(1, 1))
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int ViewCount { get; set; }
    public DateTime UpdateAt { get; set; }
}

