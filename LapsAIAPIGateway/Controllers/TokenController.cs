using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using LapsAIApiGateWay.Services;
using LapsAIDAO.Models;

namespace LapsAIApiGateWay.Controllers
{
    // A simple model for login credentials
    public class LoginModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

    [ApiController]
    public class TokenController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public TokenController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpPost("api/token")]
        public IActionResult GenerateToken([FromBody] UserInfo userinfo)
        {
            // For demonstration, use hardcoded credentials.
            // In a real application, you would validate against a database.
            //if (userinfo.Username == "test" && userinfo.Password == "password")
            {
                var jwtSettings = _configuration.GetSection("Jwt").Get<JwtSettings>();
                if (jwtSettings == null || string.IsNullOrEmpty(jwtSettings.Key) || string.IsNullOrEmpty(jwtSettings.Issuer) || string.IsNullOrEmpty(jwtSettings.Audience))
                {
                    return StatusCode(500, "JWT settings are not configured properly.");
                }

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                // Create claims for the token
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, userinfo.Username),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, userinfo.Username)
                };

                var token = new JwtSecurityToken(
                    issuer: jwtSettings.Issuer,
                    audience: jwtSettings.Audience,
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(120),
                    signingCredentials: credentials);

                var tokenHandler = new JwtSecurityTokenHandler();
                return Ok(new { token = tokenHandler.WriteToken(token) });
            }

            return Unauthorized("Invalid credentials.");
        }
    }
}
