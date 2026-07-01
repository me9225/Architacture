using System.ComponentModel.DataAnnotations;

namespace BsdFinalProject.DTOs
{
    public class ManegerDto
    {
        public int Id { get; set; }

        [MaxLength(200)]
        public string Name { get; set; }

        // model currently defines Password as int; keep same type to match DB
        public string Password { get; set; }
    }
}