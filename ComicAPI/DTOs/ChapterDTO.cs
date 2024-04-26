using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ComicApp.Data;


public class ChapterDTO
{
    public int ID { get; set; } // Primary Key (implicitly set by IDENTITY(1, 1))
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int? ChapterNumber { get; set; }
    public int ViewCount { get; set; }
    public DateTime UpdateAt { get; set; }
}

