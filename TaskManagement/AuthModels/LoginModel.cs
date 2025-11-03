using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TaskManagement.AuthModels;

public class LoginModel
{

    [Required]

    public string Username { get; set; } = string.Empty;

    [Required]
    [PasswordPropertyText]
    public string Password { get; set; } = string.Empty;
}
public class RegisterModel
{
    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    [PasswordPropertyText]
    public string Password { get; set; } = string.Empty;

    [Required]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    public string LastName { get; set; } = string.Empty;
}