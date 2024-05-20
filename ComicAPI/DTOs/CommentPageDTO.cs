using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ComicApp.Data;
using ComicApp.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;



namespace ComicApp.Models
{
    public class CommentPageDTO
    {
        public int total { get; set; } = 0;
        public int cerrent { get; set; } = 0;
        public List<CommentDTO>? Comments { get; set; }
    }
}