using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using ComicAPI.Enums;
using ComicAPI.Services;
using ComicApp.Data;
using ComicApp.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


public class ComicDTO
{
    public ComicDTO() => Type = false;
    public ComicDTO(Comic? x)
    {
        if (x == null) return;
        ID = x.ID;
        Title = x.Title;
        OtherName = x.OtherName;
        Url = x.Url;
        Author = x.Author;
        Description = x.Description;
        CoverImage = x.CoverImage;
        Status = x.Status;
        Rating = x.Rating;
        ViewCount = x.ViewCount;
        UpdateAt = x.UpdateAt;
        NumChapter = x.numchapter;
        genres = x.Genres.Select(g => new GenreLiteDTO(g));
        Chapters = x.Chapters.Select(c => new ChapterDTO(c));
        Type = x.Status == (int)ComicStatus.Ongoing && x.UpdateAt >= DateTime.UtcNow.AddMonths(-6);
    }
    public ComicDTO(Comic? x, UrlService urlService) : this(x) => CoverImage = urlService.GetComicCoverImagePath(CoverImage);
    public int ID { get; set; } // Primary Key (implicitly set by IDENTITY(1, 1))
    public string Title { get; set; } = "";
    public string OtherName { get; set; } = "";
    public string Url { get; set; } = "";
    public string? Author { get; set; }
    public string? Description { get; set; }
    public string? CoverImage { get; set; }
    public int Status { get; set; } = 0;   // Enforced by data annotation check constraint
    public float Rating { get; set; } = 10;// Enforced by data annotation check constraint
    public int ViewCount { get; set; } = 0; // view
    public DateTime UpdateAt { get; set; } = DateTime.Now;
    public int NumChapter { get; set; } = 0;
    public IEnumerable<GenreLiteDTO> genres { get; set; } = [];

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public IEnumerable<object> Chapters { get; set; } = [];

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public bool IsFollow { get; set; } = false;
    public bool Type { get; set; }
}
