using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComicApp.Models
{
    public class Chapter 
    {
        public int ID { get; set; } // Primary Key (implicitly set by IDENTITY(1, 1))
        public int? ComicID { get; set; } // Foreign Key
        public string Title { get; set; } = string.Empty;
        public int? ChapterNumber { get; set; }
        public string Content { get; set; } = string.Empty;
        public int ViewCount { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime UpdateAt { get; set; } = DateTime.Now;
        public virtual List<Page>? Pages { get; set; }
    }

}
