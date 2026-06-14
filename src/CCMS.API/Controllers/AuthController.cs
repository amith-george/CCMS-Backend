using System.Threading.Tasks;
using CCMS.Application.DTOs;
using CCMS.Application.Interfaces;
using CCMS.Infrastructure.Persistence;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CCMS.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IAuditLogService _auditLogService;

        public AuthController(
            ApplicationDbContext context, 
            IJwtTokenGenerator jwtTokenGenerator,
            IAuditLogService auditLogService)
        {
            _context = context;
            _jwtTokenGenerator = jwtTokenGenerator;
            _auditLogService = auditLogService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            // 1. Find user by email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (user == null)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            // 2. Verify password using BCrypt
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            // 3. Generate JWT Token
            var token = _jwtTokenGenerator.GenerateToken(user);

            // 4. Log the login activity
            await _auditLogService.LogAsync("Login", "User", $"User {user.Email} logged in.", null);

            // 5. Return the response
            var response = new LoginResponse
            {
                Token = token,
                Email = user.Email,
                Role = user.Role
            };

            return Ok(response);
        }
    }
}
