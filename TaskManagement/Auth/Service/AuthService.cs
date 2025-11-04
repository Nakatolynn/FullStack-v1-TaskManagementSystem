using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManagementAPI.Data;
using TaskManagementAPI.User;
using Microsoft.EntityFrameworkCore;
using TaskManagement.AuthModels;

namespace TaskManagement.Auth;

public class AuthService
{
    private readonly AppDataContext _context;
    private readonly IConfiguration _config;

    public AuthService(AppDataContext context, IConfiguration config)
    {
        _context = context;
        _config = config;
    }

    public async Task<Users?> RegisterAsync(RegisterModel model)
    {
        if (_context.Users.Any(u => u.Username == model.Username))
            return null;

        var user = new Users
        {
            Username = model.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
            FirstName = model.FirstName,
            LastName = model.LastName
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
        return user;
    }

    public LoginResponse? Login(LoginModel model)
    {
        var user = _context.Users.FirstOrDefault(u => u.Username == model.Username);
        if (user == null || !BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
            return null;

        var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]!);
        var tokenHandler = new JwtSecurityTokenHandler();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
            new Claim("UserId", user.UserId.ToString()),
            new Claim(ClaimTypes.Name, user.Username)
        }),
            Expires = DateTime.UtcNow.AddMinutes(30),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(key),
                SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);

        return new LoginResponse
        {
            Token = tokenHandler.WriteToken(token),
            UserId = user.UserId,
            Username = user.Username,
            FirstName = user.FirstName,
            LastName = user.LastName
        };
    }


    public async Task<Users?> GetUserByIdAsync(Guid userId)
    {
        if (userId == Guid.Empty)
            return null;
        return await _context.Users.FirstOrDefaultAsync(u => u.UserId == userId);
    }

}
