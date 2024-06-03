using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ComicApp.Data;
using ComicApp.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


public class ListComicDTO
{
    public List<ComicDTO>? comics { get; set; }
    public int Page { get; set; }
    public int Step { get; set; }
    public int totalpage { get; set; }

}
