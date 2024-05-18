
using System.ComponentModel.DataAnnotations;

public class UserRegisterDTO
{
    [Required]
    [EmailAddress]
    public string? email { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string? password { get; set; }
    [Required]
    [DataType(DataType.Text)]
    public string? name { get; set; }
}