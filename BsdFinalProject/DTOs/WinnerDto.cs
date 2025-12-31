using System.ComponentModel.DataAnnotations;

namespace BsdFinalProject.DTOs
{
    public class WinnerDto
    {
        public int Id { get; set; }

        public int IdUser { get; set; }

        public int IdGift { get; set; }
    }
}