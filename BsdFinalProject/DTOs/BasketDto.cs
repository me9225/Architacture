using System.ComponentModel.DataAnnotations;

namespace BsdFinalProject.DTOs
{
    public class BasketDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int GiftId { get; set; }
    }
}