using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ComicAPI.Enums;

namespace ComicAPI.Classes
{
    public class ComicQuerySearchAdvance
    {
        public int Page { get; set; } = 1;
        public int Step { get; set; } = 10;

        public ComicStatus Status { get; set; } = ComicStatus.All;
        public SortType Sort { get; set; } = SortType.TopAll;
        public string? Genres { get; set; } = null;
        public string? Notgenres { get; set; } = null;
        public int? Year { get; set; } = null;
        public string Keyword { get; set; } = "";
    }
}