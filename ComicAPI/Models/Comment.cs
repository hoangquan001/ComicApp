using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ComicApp.Data;

namespace ComicApp.Models
{
    public class Comment
    {
        [Key]
        [Column("id")]
        public int ID { get; set; }
        [Column("chapterid")]
        public int ChapterID { get; set; }
        [Column("comicid")]
        public int ComicID { get; set; }
        [Column("userid")]
        public int UserID { get; set; }
        [Column("content")]
        public string? Content { get; set; }
        [Column("commentedat")]
        public DateTime CommentedAt { get; set; } = DateTime.UtcNow;
        [Column("parentcommentid")]
        public int? ParentCommentID { get; set; }
        public User? User { get; set; }
        public Chapter? Chapter { get; set; }
        public List<Comment> Replies { get; set; } = new List<Comment>();
    }
}