namespace BsdFinalProject.Models
{
    public class Card
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int GiftId { get; set; }
        public DateTime BuingDate { get; set; }

        public Gift Gift { get; set; }
        public User User { get; set; }
    }
}
