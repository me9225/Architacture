using System.ComponentModel.DataAnnotations;

namespace BsdFinalProject.DTOs
{
    public class CreateUserDto
    {
        [Required, EmailAddress, MaxLength(50)]
        public string EMail { get; set; }

        [Required, MinLength(6)]
        public string Password { get; set; }

        [Required, MaxLength(100)]
        public string FullName { get; set; }

        [MaxLength(10)]
        public string Phone { get; set; }

        [MaxLength(50)]
        public string Address { get; set; }
    }
}