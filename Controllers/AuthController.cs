using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TaskTracker.API.Data;
using TaskTracker.API.DTOs;
using TaskTracker.API.Models;

namespace TaskTracker.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(AppDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        [HttpPost("register")]
        public async Task<ActionResult> RegisterUser(RegisterDto register)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == register.UserName);

            if (user is not null)
                return BadRequest($"user with name {register.UserName} already exists");

            var hashPassword = BCrypt.Net.BCrypt.HashPassword(register.Password);

            var newUser = new User
            {
                UserName = register.UserName,
                PasswordHash = hashPassword,
                Role = register.Role
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            return Ok("User registered successfully");
        }

        [HttpPost("login")]
        public async Task<ActionResult> LoginUser(LoginDto login)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == login.UserName);

            if (user is null || !BCrypt.Net.BCrypt.Verify(login.Password, user.PasswordHash))
                return Unauthorized("invalid user name or password");

            var jwtSettings = _config.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"];
            var issuer = jwtSettings["Issuer"];
            var audience = jwtSettings["Audience"];
            var expiry = login.RememberMe ? 10080 : int.Parse(jwtSettings["ExpiryInMinutes"]!);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier,user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                        issuer: issuer,
                        audience: audience,
                        claims: claims,
                        expires: DateTime.UtcNow.AddMinutes(expiry),
                        signingCredentials: credentials);

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new { token = tokenString });
        }

    }
}
