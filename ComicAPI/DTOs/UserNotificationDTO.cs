using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ComicApp.Models;

namespace ComicAPI.DTOs
{
    public class UserNotificationDTO
    {


        public int ID { get; set; }

        public int UserID { get; set; }

        public int ComicID { get; set; }

        public string NotificationContent { get; set; } = "";

        public DateTime NotificationTimestamp { get; set; } = DateTime.UtcNow;

        public bool IsRead { get; set; } = false;

        private string? _CoverImage;
        public string? CoverImage
        {
            get
            {
                return _CoverImage;
            }
            set
            {
                _CoverImage = "http://localhost:5080/static/CoverImg/" + value;
            }
        }
        public string URLComic { get; set; } = string.Empty;
        public int? lastchapter { get; set; }
        public string URLChapter { get; set; } = string.Empty;
    }
}