using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ComicAPI.DTOs
{
    public class UpdateUserInfo
    {
        public string? Email { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public DateTime? Dob { get; set; }
    }
}