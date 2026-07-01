namespace BsdFinalProject.Models
{
    public class Gift
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int Cost { get; set; }
        public string Picture { get; set; }
        public int CategoryId { get; set; }
        public int DonorId { get; set; }
        public List<Card> CardsList { get; set; }
        public string WinnerName { get; set; }
        public Donor Donor { get; set; }
        public Category Category { get; set; }


    }
}
