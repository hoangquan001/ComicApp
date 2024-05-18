using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ComicApp.Data;
using ComicApp.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;



namespace ComicApp.Models
{
    public class CommentDTO
    {

        public int ID { get; set; }

        public int ChapterID { get; set; }

        public int ComicID { get; set; }

        public int UserID { get; set; }
        public string? Content { get; set; }

        public DateTime CommentedAt { get; set; }

        public string? UserName { get; set; }
        public int? ParentCommentID { get; set; }
        public List<CommentDTO>? Replies { get; set; }
    }
}