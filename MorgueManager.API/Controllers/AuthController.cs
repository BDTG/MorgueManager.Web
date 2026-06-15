using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace MorgueManager.API.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;

    public AuthController(IConfiguration config)
    {
        _config = config;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        if (string.IsNullOrEmpty(request.Email) || string.IsNullOrEmpty(request.AccessKey))
        {
            return BadRequest(new { Message = "Email và Access Key là bắt buộc." });
        }

        if (request.AccessKey != "MM-ACTIVE-2026")
        {
            return Unauthorized(new { Message = "Access Key không hợp lệ." });
        }

        // Determine role based on email prefix
        string role = "Staff";
        if (request.Email.StartsWith("admin", StringComparison.OrdinalIgnoreCase))
        {
            role = "Admin";
        }
        else if (request.Email.StartsWith("manager", StringComparison.OrdinalIgnoreCase))
        {
            role = "Manager";
        }

        var token = GenerateJwtToken(request.Email, role);

        return Ok(new
        {
            Token = token,
            Email = request.Email,
            Role = role,
            DisplayName = role + " User"
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
