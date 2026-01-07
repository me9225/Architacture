using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace BsdFinalProject.DTOs
{
    public class CreateDonorDto
    {
        [MaxLength(100)]
        public string Name { get; set; }

        [Required, EmailAddress, MaxLength(50)]
        public string Email { get; set; }
    }
}