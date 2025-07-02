using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnalysisAPI.Models
{
    public class Analysis
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [Required]
        public InputType Type { get; set; }

        public string? InputText { get; set; }

        public string? Url { get; set; }

        public User? User { get; set; }

        public List<SentimentDistribution> SentimentDistribution { get; set; } = new();
    }

}
