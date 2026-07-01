using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BsdFinalProject.DTOs
{
    public class GiftDto
    {
        [Required]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(300)]
        public string Description { get; set; }

        [Required,DefaultValue(20),]
        [Range(0, 100000)]
        public int Cost { get; set; }

        [MaxLength(300)]
        public string Picture { get; set; }
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        [ForeignKey("Donor")]
        public int DonorId { get; set; }
        [MaxLength(100),DefaultValue("")]
        public string WinnerName { get; set; }
    }
    public class GiftDtoWithSum
    {
      

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(300)]
        public string Description { get; set; }

        [Required, DefaultValue(20),]
        [Range(0, 100000)]
        public int Cost { get; set; }

        [MaxLength(300)]
        public string Picture { get; set; }
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        [ForeignKey("Donor")]
        public int DonorId { get; set; }
        [MaxLength(100), DefaultValue("")]

        public string WinnerName { get; set; }
        public int Count { get; set; }
    }
}