using System.ComponentModel.DataAnnotations;

namespace   BsdFinalProject.DTOs
{
    public class CreateGiftDto
    {
        [Required, MaxLength(30)]
        public string Name { get; set; }

        [MaxLength(300)]
        public string Description { get; set; }

        [Range(0, 100000)]
        public int Cost { get; set; }

        [MaxLength(200)]
        public string Picture { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int DonorId { get; set; }
    }
}