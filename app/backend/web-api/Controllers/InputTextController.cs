using AnalysisAPI.Models;
using AnalysisAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AnalysisAPI.Data;
using System.Threading.Tasks;
using System;

namespace AnalysisAPI.Controllers
{
    [ApiController]
    [Route("sentiment/text")]
    public class InputTextController : SentimentController
    {
        private readonly PythonService _pythonService;

        public InputTextController(PythonService pythonService, AppDbContext context)
            : base(context)
        {
            _pythonService = pythonService;
        }

        [HttpPost("analyze")]
        public async Task<IActionResult> AnalyzeSentiment([FromBody] string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return BadRequest(new { message = "Input is required" });
            }

            try
            {
                var analysisEntity = new Analysis
                {
                    UserId = CurrentUserId,
                    Type = InputType.Text,
                    InputText = text
                };

                _dbContext.Analyses.Add(analysisEntity);
                await _dbContext.SaveChangesAsync();

                string analysisResult = await _pythonService.RunPythonScript(text);
                int sentimentScore = int.Parse(analysisResult);

                var sentimentDistribution = new SentimentDistribution
                {
                    AnalysisId = analysisEntity.Id,
                    Label = Enum.GetName(typeof(SentimentType), sentimentScore) ?? "Unknown",
                    Percentage = 100
                };

                _dbContext.SentimentDistributions.Add(sentimentDistribution);

                await _dbContext.SaveChangesAsync();

                return Ok(new
                {
                    sentimentDistribution.Label,
                    sentimentDistribution.Percentage
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing the sentiment analysis.", error = ex.Message, stackTrace = ex.StackTrace });
            }
        }
    }
}
