using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using AnalysisAPI.Services;
using AnalysisAPI.Models;
using AnalysisAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace AnalysisAPI.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly AppDbContext _dbContext;

        public AuthController(AuthService authService, AppDbContext dbContext)
        {
            _authService = authService;
            _dbContext = dbContext;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authService.RegisterUser(user);
            if (!result.Success)
                return BadRequest(new { message = result.Message });

            return Ok(new
            {
                message = "Registration successful",
                username = user.Username,
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO loginCredentials)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var token = await _authService.ValidateUser(loginCredentials.Username, loginCredentials.Password);
            if (token == null)
                return Unauthorized(new { message = "Invalid username or password" });

            var dbUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == loginCredentials.Username);
            if (dbUser == null)
                return Unauthorized(new { message = "Invalid username or password" });

            return Ok(new
            {
                message = "Login successful",
                username = dbUser.Username,
                id = dbUser.Id,
                token = token
            });
        }

        [HttpPost("logout")]
        [Authorize]
        public IActionResult Logout()
        {
            return Ok(new { message = "Logged out successfully" });
        }

    }
}
