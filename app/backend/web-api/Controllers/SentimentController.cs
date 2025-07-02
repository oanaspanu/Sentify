using AnalysisAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AnalysisAPI.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;

namespace AnalysisAPI.Controllers
{
    [ApiController]
    [Route("sentiment")]
    public class SentimentController : ControllerBase
    {
        protected readonly AppDbContext _dbContext;

        protected int CurrentUserId => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        public SentimentController(AppDbContext context)
        {
            _dbContext = context;
        }

        [HttpGet("{analysisId}")]
        public async Task<IActionResult> GetSentiment(int analysisId)
        {
            var analysisEntity = await _dbContext.Analyses
                .Where(a => a.Id == analysisId && a.UserId == CurrentUserId)
                .FirstOrDefaultAsync();

            if (analysisEntity == null)
            {
                return NotFound(new { message = "Sentiment result not found or you do not have permission to view this." });
            }

            var sentimentDistributions = await _dbContext.SentimentDistributions
                .Where(s => s.AnalysisId == analysisId)
                .ToListAsync();

            return Ok(new
            {
                Source = analysisEntity.Type == InputType.Text ? analysisEntity.InputText : analysisEntity.Url,

                SentimentDetails = sentimentDistributions
                    .Select(s => new
                    {
                        Label = s.Label.ToString(),
                        Percentage = s.Percentage
                    })
                    .ToList()
            });
        }


        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUserSentiments(int userId)
        {
            if (userId != CurrentUserId)
            {
                return Unauthorized(new { message = "You are not authorized to view another user's data." });
            }

            var userAnalyses = await _dbContext.Analyses
                .Where(a => a.UserId == userId)
                .ToListAsync();

            if (!userAnalyses.Any())
            {
                return Ok(new List<object>());
            }

            var sentimentDistributions = await _dbContext.SentimentDistributions
                .Where(s => userAnalyses.Select(a => a.Id).Contains(s.AnalysisId))
                .ToListAsync();

            var result = userAnalyses.Select(a => new
            {
                AnalysisId = a.Id,
                Source = a.InputText != null ? a.InputText : a.Url,
                SentimentDetails = sentimentDistributions
                    .Where(s => s.AnalysisId == a.Id)
                    .Select(s => new
                    {
                        Label = s.Label.ToString(),
                        Percentage = s.Percentage
                    })
                    .ToList()
            }).ToList();

            return Ok(result);
        }


        [HttpDelete("{analysisId}")]
        public async Task<IActionResult> DeleteSentiment(int analysisId)
        {
            var analysisEntity = await _dbContext.Analyses
                .Where(a => a.Id == analysisId && a.UserId == CurrentUserId)
                .FirstOrDefaultAsync();

            if (analysisEntity == null)
            {
                return NotFound(new { message = "Sentiment analysis not found or you do not have permission to delete it." });
            }

            var sentimentDistributions = await _dbContext.SentimentDistributions
                .Where(s => s.AnalysisId == analysisId)
                .ToListAsync();

            _dbContext.SentimentDistributions.RemoveRange(sentimentDistributions);
            _dbContext.Analyses.Remove(analysisEntity);

            await _dbContext.SaveChangesAsync();

            return Ok(new { message = "Sentiment analysis deleted successfully." });
        }
    }
}
