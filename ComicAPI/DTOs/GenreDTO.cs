using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ComicApp.Data;


public class GenreDTO
{
    public int ID { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
