using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ComicApp.Data;
using ComicApp.Models;


public class GenreLiteDTO
{
    public GenreLiteDTO() { }
    public GenreLiteDTO(Genre genre)
    {
        ID = genre.ID;
        Title = genre.Title;
    }
    public int ID { get; set; }
    public string Title { get; set; } = string.Empty;

}
