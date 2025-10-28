using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskManagement.Auth;

[Route("api/[controller]")]
[ApiController]
public class AuthManagementController : ControllerBase
{
    private readonly IConfiguration _configuration;
    private readonly AuthService _authService;

    public AuthManagementController(IConfiguration configuration, AuthService authService)
    {
        _configuration = configuration;
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        var user = await _authService.RegisterAsync(model);
        if (user == null)
            return BadRequest("Username already exists.");
        return Ok(new { user.UserId, user.Username });
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginModel model)
    {
        var token = _authService.Login(model);
        if (token == null)
            return Unauthorized("Invalid credentials");
        return Ok(new { token });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUserById(string id)
    {
        if (!Guid.TryParse(id, out var guid))
            return BadRequest(new { error = "'id' must be a GUID." });

        var user = await _authService.GetUserByIdAsync(guid);
        if (user == null)
            return NotFound();
        return Ok(new { user.UserId, user.Username, user.FirstName, user.LastName });
    }
}
