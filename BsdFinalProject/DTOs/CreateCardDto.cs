using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BsdFinalProject.DTOs
{
    public class CreateCardDto
    {
        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }

        [Required]
        [ForeignKey("Gift")]
        public int GiftId { get; set; }

        [Required]
        public DateTime BuingDate { get; set; }
    }
}