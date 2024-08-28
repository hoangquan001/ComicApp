using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using ComicApp.Models;

namespace ComicAPI.Models
{
    public class UserNotification
    {
        [Key]
        [Column("id")]
        public int ID { get; set; }
        [Column("userid")]
        public int UserID { get; set; }
        [Column("comicid")]
        public int ComicID { get; set; }
        [Column("content")]
        public string Content { get; set; } = "";
        [Column("timestamp")]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        [Column("isread")]
        public bool IsRead { get; set; } = false;
        [Column("coverimage")]
        public string CoverImage { get; set; } = string.Empty;
        [Column("urlcomic")]
        public string URLComic { get; set; } = string.Empty;
        [Column("lastchapter")]
        public int? lastchapter { get; set; }
        [Column("urlchapter")]
        public string URLChapter { get; set; } = string.Empty;


    }
}