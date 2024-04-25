using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ComicApp.Models
{
    public class Chapter
    {
        [Column("id")]
        public int ID { get; set; } // Primary Key (implicitly set by IDENTITY(1, 1))

        [Column("comicid")]
        public int? ComicID { get; set; } // Foreign Key

        [Column("title")]
        public string Title { get; set; } = string.Empty;

        [Column("chapternumber")]
        public int? ChapterNumber { get; set; }

        [Column("viewcount")]
        public int ViewCount { get; set; }
        [Column("updateat")]
        public DateTime UpdateAt { get; set; }
        public virtual List<Page>? Pages { get; set; }
    }

}

