using System.ComponentModel.DataAnnotations;

namespace BsdFinalProject.DTOs
{
    public class DonorDto
    {
        [Required]
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [EmailAddress, MaxLength(50)]
        public string Email { get; set; }
    }
}