using System.ComponentModel.DataAnnotations;

namespace BsdFinalProject.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }

        [MaxLength(20)]
        public string Name { get; set; }
    }
}