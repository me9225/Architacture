using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BsdFinalProject.DTOs
{
    public class CardDto
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [ForeignKey("User")]
        public int UserId { get; set; }
        [Required]
        [ForeignKey("Gift")]
        public int GiftId { get; set; }

        public DateTime BuingDate { get; set; }
    }
    public class CardSumDto
    {
        [Required]
        [ForeignKey("Gift")]
        public int GiftId { get; set; }

        public int Count { get; set; }
    }
}