using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace ComicAPI.DTOs
{
    public class UpdateUserPassword
    {
        [Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; } = "";
        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; } = "";
        [Required]
        [DataType(DataType.Password)]
        public string? RePassword { get; set; } = "";
    }
}