using ApiAuth.Data;
using ApiAuth.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

[Route("api/[controller]")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IConfiguration _configuration;

    public AuthenticationController(ApplicationDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login([FromBody] UserLoginModel userLogin)
    {
        var user = await _context.Users
                                 .FirstOrDefaultAsync(u => u.Username == userLogin.Username);
        if (user == null)
        {
            return Unauthorized(new { message = "Invalid username or password" });
        }

        bool isPasswordValid = BCrypt.Net.BCrypt.Verify(userLogin.Password, user.PasswordHash);
        if (!isPasswordValid)
        {
            return Unauthorized(new { message = "Invalid username or password" });
        }

        var roles = await _context.UserRoles
    .Where(ur => ur.UserId == user.UserId)
    .Join(_context.Roles,
        ur => ur.RoleId,
        r => r.RoleId,
        (ur, r) => r.RoleName)
    .ToListAsync();

        Console.WriteLine($"Roles: {string.Join(", ", roles)}");
        var token = GenerateJwtToken(user, roles);
        return Ok(new { Token = token });
    }

    private string GenerateJwtToken(User user, List<string> roles)
    {
        var secretKey = _configuration["Jwt:SecretKey"];
        var issuer = _configuration["Jwt:Issuer"];
        var audience = _configuration["Jwt:Audience"];

        if (string.IsNullOrEmpty(secretKey) || string.IsNullOrEmpty(issuer) || string.IsNullOrEmpty(audience))
        {
            throw new InvalidOperationException("Missing configuration values");
        }

        var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString())
    };

        if (roles != null && roles.Count > 0)
        {
            foreach (var role in roles)
            {
                //Console.WriteLine($"Test: {role}");
                claims.Add(new Claim(ClaimTypes.Role, role));
            }
        }
        else
        {
            Console.WriteLine($"Role is null");
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.Now.AddMinutes(5),  // Set the token expiration to 5 minutes
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}