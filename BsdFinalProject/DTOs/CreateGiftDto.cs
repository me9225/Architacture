using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace   BsdFinalProject.DTOs
{
    public class CreateGiftDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(300)]
        public string Description { get; set; }

        [Range(0, 100000),DefaultValue(20)]
        public int Cost { get; set; }

        [MaxLength(300)]
        public string Picture { get; set; }

        [Required]
        [ForeignKey("Category")]
        public int CategoryId { get; set; }

        [Required]
        [ForeignKey("Donor")]
        public int DonorId { get; set; }
    }
}