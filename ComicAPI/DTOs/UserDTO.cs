
public class UserDTO
{
    public int ID { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    private string? _Avatar;
    public string? Avatar
    {
        get
        {
            return _Avatar;
        }
        set
        {
            _Avatar = "http://localhost:5080/static/Avatarimg/" + value;
        }
    }
    public DateTime? Dob { get; set; }
    public int Gender { get; set; }
    public string? Token { get; set; }
}