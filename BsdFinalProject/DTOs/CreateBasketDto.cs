using System.ComponentModel.DataAnnotations;

namespace BsdFinalProject.DTOs
{
    public class CreateBasketDto
    {
        [Required]
        public int UserId { get; set; }

        [Required]
        public int GiftId { get; set; }
    }
}