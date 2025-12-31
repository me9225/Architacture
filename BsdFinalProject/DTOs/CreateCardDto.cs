using System.ComponentModel.DataAnnotations;

namespace BsdFinalProject.DTOs
{
    public class CreateCardDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int GiftId { get; set; }

        [Required]
        public DateTime BuingDate { get; set; }
    }
}