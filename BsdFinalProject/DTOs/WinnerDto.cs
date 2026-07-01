using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BsdFinalProject.DTOs
{
    public class WinnerDto
    {
        [Required]
        public int Id { get; set; }

        [Required,ForeignKey("User")]
        public int IdUser { get; set; }
        [Required,ForeignKey("Gift")]
        public int IdGift { get; set; }
    }
}