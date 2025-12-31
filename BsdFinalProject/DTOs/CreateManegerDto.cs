using System.ComponentModel.DataAnnotations;

namespace BsdFinalProject.DTOs
{
    public class CreateManegerDto
    {
        [Required, MaxLength(200)]
        public string Name { get; set; }

        [Required]
        public string Password { get; set; }
    }
}