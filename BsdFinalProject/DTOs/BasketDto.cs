using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BsdFinalProject.DTOs
{
    public class BasketDto
    {
        [Required]
        
        public int Id { get; set; }
        [Required]
        //אם זה עושה בעיות אפשר להוריד את ה- ForeignKey
        [ForeignKey("User")]
        public int UserId { get; set; }
        [Required]
        //אם זה עושה בעיות אפשר להוריד את ה- ForeignKey
        [ForeignKey("Gift")]
        public int GiftId { get; set; }
    }
}