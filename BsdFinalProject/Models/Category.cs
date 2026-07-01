namespace BsdFinalProject.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Gift> GiftsList { get; set; }
    }
}
