using System.ComponentModel.DataAnnotations;

namespace BsdFinalProject.DTOs
{
    public class UserDto
    {
        public int Id { get; set; }

        [EmailAddress, MaxLength(50)]
        public string EMail { get; set; }

        [MaxLength(100)]
        public string FullName { get; set; }

        [MaxLength(10)]
        public string Phone { get; set; }

        [MaxLength(50)]
        public string Address { get; set; }
    }
}