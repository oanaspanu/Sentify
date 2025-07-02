using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AnalysisAPI.Models
{
    public class SentimentDistribution
    {

        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Analysis")]
        public int AnalysisId { get; set; }

        [Required]
        public string Label { get; set; } = string.Empty;

        [Required]
        // in case of an input text, the percentage is 100% of one label and 0% of the others
        public double Percentage { get; set; } = 0.0;

        public Analysis? Analysis { get; set; }
    }
}
