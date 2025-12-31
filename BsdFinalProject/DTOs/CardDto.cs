using System.ComponentModel.DataAnnotations;

namespace BsdFinalProject.DTOs
{
    public class CardDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int GiftId { get; set; }

        public DateTime BuingDate { get; set; }
    }
}