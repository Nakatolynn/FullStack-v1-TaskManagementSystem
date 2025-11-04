namespace TaskManagement.AuthModels;

public class LoginResponse
{
    public string Token { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public string Username { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}
