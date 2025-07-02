using AnalysisAPI.Models;
using AnalysisAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AnalysisAPI.Data;
using System.Threading.Tasks;
using System.Linq;
using System;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Security.Claims;

namespace AnalysisAPI.Controllers
{
    [ApiController]
    [Route("sentiment/youtube")]
    public class YouTubeSentimentController : SentimentController
    {
        private readonly PythonService _pythonService;
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;
        private int CurrentUserId => int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

        public YouTubeSentimentController(
            PythonService pythonService,
            AppDbContext context,
            HttpClient httpClient
        ) : base(context)
        {
            _pythonService = pythonService;
            _httpClient = httpClient;
            _apiKey = Environment.GetEnvironmentVariable("YOUTUBE_API_KEY")
                ?? throw new Exception("YOUTUBE_API_KEY not found in environment variables.");
        }

        [HttpPost("analyze")]
        public async Task<IActionResult> AnalyzeSentiment([FromBody] string videoUrl)
        {
            if (string.IsNullOrWhiteSpace(videoUrl))
                return BadRequest(new { message = "Video URL is required" });

            try
            {
                if (!IsYouTubeUrl(videoUrl))
                    return BadRequest(new { message = "Invalid YouTube URL" });

                var comments = await GetYoutubeCommentsAsync(videoUrl);

                if (comments.Count == 0)
                    return NotFound(new { message = "No comments found for this YouTube video." });

                var analysisEntity = new Analysis
                {
                    UserId = CurrentUserId,
                    Type = InputType.Youtube,
                    Url = videoUrl
                };

                _dbContext.Analyses.Add(analysisEntity);
                await _dbContext.SaveChangesAsync();

                var sentimentCounts = new Dictionary<string, int>
                {
                    { "Negative", 0 },
                    { "Neutral", 0 },
                    { "Positive", 0 }
                };

                foreach (var commentText in comments)
                {
                    var result = await _pythonService.RunPythonScript(commentText);
                    int sentimentScore = int.Parse(result);
                    var label = Enum.GetName(typeof(SentimentType), sentimentScore) ?? "Unknown";
                    sentimentCounts[label]++;
                }

                int total = sentimentCounts.Values.Sum();

                foreach (var kvp in sentimentCounts)
                {
                    if (kvp.Value == 0) continue;

                    var sentimentDistribution = new SentimentDistribution
                    {
                        AnalysisId = analysisEntity.Id,
                        Label = kvp.Key,
                        Percentage = Math.Round((kvp.Value / (double)total) * 100, 2)
                    };

                    _dbContext.SentimentDistributions.Add(sentimentDistribution);
                }

                await _dbContext.SaveChangesAsync();

                return Ok(new
                {
                    message = "YouTube sentiment analysis complete.",
                    distribution = sentimentCounts.ToDictionary(
                        kvp => kvp.Key,
                        kvp => Math.Round((kvp.Value / (double)total) * 100, 2)
                    )
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Error during YouTube sentiment analysis.",
                    error = ex.Message,
                    stack = ex.StackTrace
                });
            }
        }

        // Validate YouTube URL
        private bool IsYouTubeUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var uri)
                && (uri.Host.Contains("youtube.com") || uri.Host.Contains("youtu.be"));
        }

        // Extract video ID from standard or short URL
        private string ExtractVideoIdFromUrl(string url)
        {
            var uri = new Uri(url);
            if (uri.Host.Contains("youtu.be"))
                return uri.AbsolutePath.TrimStart('/');

            var query = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(uri.Query);
            return query.TryGetValue("v", out var id) ? id.ToString() : throw new Exception("Invalid YouTube URL format");
        }

        private async Task<List<string>> GetYoutubeCommentsAsync(string videoUrl)
        {
            var videoId = ExtractVideoIdFromUrl(videoUrl);
            var apiUrl = $"https://www.googleapis.com/youtube/v3/commentThreads?part=snippet&videoId={videoId}&key={_apiKey}&maxResults=100";

            var response = await _httpClient.GetAsync(apiUrl);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var json = JsonConvert.DeserializeObject<dynamic>(content);

            var comments = new List<string>();
            foreach (var item in json.items)
            {
                comments.Add((string)item.snippet.topLevelComment.snippet.textDisplay);
            }

            return comments;
        }
    }
}
