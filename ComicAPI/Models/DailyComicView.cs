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
        public int ComicID { get; set; }
        public DateTime ViewDate { get; set; }
        public int ViewCount { get; set; }

        public Comic? comic { get; set; }

    }
}