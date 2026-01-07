using System.ComponentModel.DataAnnotations;

namespace BsdFinalProject.DTOs
{
    public class UserDto
    {
        [Required]
        public int Id { get; set; }

        [EmailAddress, MaxLength(50)]
        public string EMail { get; set; }

        [MaxLength(100),Required]
        public string FullName { get; set; }

        [MaxLength(20)]
        public string Phone { get; set; }

        [MaxLength(500)]
        public string Address { get; set; }
    }
}