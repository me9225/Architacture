namespace BsdFinalProject.Models
{
    public class Winner
    {
        public int Id { get; set; }
        public int IdUser { get; set; }
        public int IdGift { get; set; }
        public User User { get; set; }
        public Gift Gift { get; set; }
    }
}
