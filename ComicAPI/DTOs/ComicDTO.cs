using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ComicApp.Data;
using ComicApp.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


public class ComicDTO
{
    public int ID { get; set; } // Primary Key (implicitly set by IDENTITY(1, 1))
    public string Title { get; set; } = "";
    public string OtherName { get; set; } = "";
    public string Url { get; set; } = "";
    public string? Author { get; set; }
    public string? Description { get; set; }
    private string? _CoverImage;
    public int Status { get; set; } = 0;   // Enforced by data annotation check constraint
    public float Rating { get; set; } = 10;// Enforced by data annotation check constraint
    public int ViewCount { get; set; } = 0; // view
    public DateTime UpdateAt { get; set; } = DateTime.Now;
    public int NumChapter { get; set; } = 0;
    public IEnumerable<GenreLiteDTO> genres { get; set; } = [];
    public IEnumerable<object> Chapters { get; set; } = [];
    public bool IsFollow { get; set; } = false;

    public string? CoverImage
    {
        get
        {
            return _CoverImage;
        }
        set
        {
            _CoverImage = "http://localhost:5080/static/CoverImg/" + value;
        }
    }
}
