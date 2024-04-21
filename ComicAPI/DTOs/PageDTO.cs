using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ComicApp.Data;
using Microsoft.EntityFrameworkCore.Metadata.Internal;


public class PageDTO
{

    public int PageNumber { get; set; }
    public string URL { get; set; } = "";

}
