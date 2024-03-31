
using System.ComponentModel.DataAnnotations;

public class UserRegisterDTO
{
    [Required]
    [EmailAddress]
    public string? email { get; set; }
    [Required]
    public string? username { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string? password { get; set; }
}