using System.ComponentModel.DataAnnotations;

namespace BsdFinalProject.DTOs
{
    public class CreateWinnerDto
    {
        [Required]
        public int IdUser { get; set; }

        [Required]
        public int IdGift { get; set; }
    }
}