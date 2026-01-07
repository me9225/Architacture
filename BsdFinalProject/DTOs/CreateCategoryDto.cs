using System.ComponentModel.DataAnnotations;

namespace BsdFinalProject.DTOs
{
    public class CreateCategoryDto
    {
        [Required, MaxLength(100)]
        public string Name { get; set; }
    }
}