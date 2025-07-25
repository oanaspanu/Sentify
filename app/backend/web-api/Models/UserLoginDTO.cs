using System.ComponentModel.DataAnnotations;

namespace AnalysisAPI.Models
{
    public class UserLoginDTO
    {
        [Required]
        public string Username { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}