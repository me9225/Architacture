using System.ComponentModel.DataAnnotations;

namespace BsdFinalProject.DTOs
{
    public class GiftDto
    {
        public int Id { get; set; }

        [Required, MaxLength(30)]
        public string Name { get; set; }

        [MaxLength(300)]
        public string Description { get; set; }

        [Range(0, 100000)]
        public int Cost { get; set; }

        [MaxLength(200)]
        public string Picture { get; set; }

        public int CategoryId { get; set; }
        public int DonorId { get; set; }
        [MaxLength(100)]
        public string WinnerName { get; set; }
    }
}