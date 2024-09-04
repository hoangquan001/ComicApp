using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using ComicApp.Models;

namespace ComicAPI.Models
{
    public class  Notification
    {
        [Column("id")]
        [Key]
        public int ID { get; set; }

        [Column("message")]
        public string? Message { get; set; } 

        [Column("createdat")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("type")]
        public int Type { get; set; } = 0;

        [Column("comicid")]
        public int? ComicId { get; set; }

        [Column("chapterid")]
        public int? ChapterId { get; set; }

        [Column("commentid")]
        public int? CommentId { get; set; }

    }

}