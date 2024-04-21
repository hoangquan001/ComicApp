using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ComicApp.Data;


public class ChapterPageDTO: ChapterDTO
{
    public IEnumerator<PageDTO> ?Pages { get; set; } 
}

