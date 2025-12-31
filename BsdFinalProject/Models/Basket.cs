namespace BsdFinalProject.Models
{
    public class Basket
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int GiftId { get; set; }
        public User User { get; set; }
    }
}
