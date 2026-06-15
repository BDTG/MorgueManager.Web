using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using MorgueManager.API.Data;
using MorgueManager.API.Models;
using System.Linq;

namespace MorgueManager.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;
    private readonly AppDbContext _context;

    public AuthController(IConfiguration config, AppDbContext context)
    {
        _config = config;
        _context = context;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.AccessKey))
        {
            return BadRequest(new { Message = "Email và Access Key là bắt buộc." });
        }

        // Retrieve user from database
        var user = _context.Users.FirstOrDefault(u => u.Email == request.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.AccessKey, user.PasswordHash))
        {
            return Unauthorized(new { Message = "Email hoặc Access Key không chính xác." });
        }

        var token = GenerateJwtToken(user.Email, user.Role);

        return Ok(new
        {
            Token = token,
            Email = user.Email,
            Role = user.Role,
            DisplayName = user.DisplayName
        });
    }

    private string GenerateJwtToken(string email, string role)
    {
        var jwtSettings = _config.GetSection("Jwt");
        var secret = jwtSettings["Secret"] ?? "MorgueManagerSuperSecretJWTKey2026!ForTestingOnlyAndMustBeAtLeast256BitsLong!";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, email),
            new Claim(ClaimTypes.Email, email),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: jwtSettings["Issuer"] ?? "MorgueManagerAPI",
            audience: jwtSettings["Audience"] ?? "MorgueManagerWeb",
            claims: claims,
            expires: DateTime.Now.AddDays(7),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public class LoginRequest
{
    public string Email { get; set; } = "";
    public string AccessKey { get; set; } = "";
}
