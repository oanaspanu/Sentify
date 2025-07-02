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
    [Route("users")]
    public class UserController : ControllerBase
    {
        private readonly AuthService _authService;
        private readonly AppDbContext _dbContext;

        public UserController(AuthService authService, AppDbContext dbContext)
        {
            _authService = authService;
            _dbContext = dbContext;
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetUser(int id)
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null) return NotFound(new { message = "User not found" });

            return Ok(new
            {
                user.Id,
                user.Username,
                user.Email
            });
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _dbContext.Users.FindAsync(id);
            if (user == null) return NotFound(new { message = "User not found" });

            _dbContext.Users.Remove(user);
            await _dbContext.SaveChangesAsync();
            return Ok(new { message = "User deleted successfully" });
        }

        [HttpPatch("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserUpdateDTO updatedUser)
        {
            var result = await _authService.UpdateUser(id, updatedUser);
            if (!result.Success)
                return NotFound(new { message = result.Message });

            return Ok(new{message = "User updated successfully"});
        }
    }
}
