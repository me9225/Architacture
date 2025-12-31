using System.ComponentModel.DataAnnotations;

namespace BsdFinalProject.DTOs
{
    public class DonorDto
    {
        public int Id { get; set; }

        [MaxLength(20)]
        public string Name { get; set; }

        [EmailAddress, MaxLength(50)]
        public string Email { get; set; }
    }
}