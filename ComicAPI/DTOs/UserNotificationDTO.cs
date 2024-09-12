using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ComicAPI.Models;
namespace ComicAPI.DTOs
{
    public class UserNotificationDTO
    {
        public int ID { get; set; }
        public int UserID { get; set; }
        public int ComicID { get; set; }
        public string Content { get; set; } = "";
        public string Image { get; set; } = "";
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
        public int Type { get; set; } = 0;

    }
}