using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using ComicApp.Models;

namespace ComicAPI.Models
{
    public class DailyComicView
    {
        [Key]
        [Column("comicid")]
        public int ComicID { get; set; }
        [Column("viewdate")]
        [DataType(DataType.Date)]
        public DateTime ViewDate { get; set; }
        [Column("viewcount")]
        public int ViewCount { get; set; }

        public Comic? comic { get; set; }

    }
}