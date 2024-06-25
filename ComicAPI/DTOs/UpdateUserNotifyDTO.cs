using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComicAPI.DTOs
{
    public class UpdateUserNotifyDTO
    {
        public int? ID { get; set; }

        public bool? IsRead { get; set; } = null;
    }
}