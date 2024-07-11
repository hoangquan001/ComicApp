using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ComicApp.Data;
using ComicApp.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;



namespace ComicApp.Models
{
    public class CommentPageDTO
    {
        public int Totalpage { get; set; } = 0;
        public int cerrentpage { get; set; } = 0;

        public List<CommentDTO>? Comments { get; set; }
    }
}