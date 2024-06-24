using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ComicApp.Data;

namespace ComicApp.Models
{
    public class Comic
    {
        [Key]
        [Column("id")]
        public int ID { get; set; } // Primary Key (implicitly set by IDENTITY(1, 1))

        [Column("title")]
        public string Title { get; set; } = "";

        [Column("othername")]
        public string OtherName { get; set; } = "";

        [Column("url")]
        public string Url { get; set; } = "";

        [Column("author")]
        public string? Author { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Column("coverimage")]
        public string? CoverImage { get; set; }

        [Column("status")]
        public int Status { get; set; } = 0;

        [Column("rating")]
        public int Rating { get; set; } = 10;
        [Column("viewcount")]
        public int ViewCount { get; set; } = 0;

        [Column("createat")]
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;

        [Column("updateat")]
        public DateTime UpdateAt { get; set; } = DateTime.UtcNow;

        [Column("numchapter")]
        public int numchapter { get; set; }

        [Column("lastchapter")]
        public int lastchapter { get; set; }
        // public Chapter? LastChapter { get; set; }
        public List<Genre> Genres { get; set; } = new List<Genre>();
        public List<Chapter> Chapters { get; set; } = new List<Chapter>();
    }
}